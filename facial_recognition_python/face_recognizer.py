import face_recognition
from flask import jsonify

dic = {
    "male": {
        "saved_unique_identification": [],
        "saved_face_encodings": [],
    },
    "female": {
        "saved_unique_identification": [],
        "saved_face_encodings": [],
    }
}


# call detects faces method with respective data of female set or males set
def detect_faces_in_image(file_stream, uniqueString, gender):
    if gender in dic:
        saved_unique_identification = dic[gender]["saved_unique_identification"]
        saved_face_encodings = dic[gender]["saved_face_encodings"]
        return detect_faces(file_stream, uniqueString,saved_unique_identification, saved_face_encodings)
    else: return jsonify({
        "result": None,
        "saved": None,
        "error": "Gender is not valid",
    })
        

def detect_faces(file_stream, uniqueString, saved_unique_identification, saved_face_encodings): 

    # Load the uploaded image file
    img = face_recognition.load_image_file(file_stream)
    # Get face encodings for any faces in the uploaded image
    unknown_face_encodings = face_recognition.face_encodings(img)

    result = None
    error = None
    saved = False
    indexs_to_Generate_Distance_for = []

    # if the received image contain any faces then
    if len(unknown_face_encodings) > 0:
        first_unknown_face = unknown_face_encodings[0]
        # See if the first face in the uploaded image matches the known faces
        match_results = face_recognition.compare_faces(saved_face_encodings, first_unknown_face)

        # Searching in the faces dic to see if the face match any of them
        for index,val in enumerate(match_results):
            if match_results[index]:
                indexs_to_Generate_Distance_for.append(index)
        
        # If matches were found then we calculate distance for them
        # and add them to the result array in this style [code, distance]
        if len(indexs_to_Generate_Distance_for) > 0:
            result = []
            for i in indexs_to_Generate_Distance_for:
                sub_result = []
                sub_result.append(saved_unique_identification[i])
                distance = face_recognition.face_distance([saved_face_encodings[i]], first_unknown_face)
                sub_result.append(distance[0])
                result.append(sub_result)

        # else if was empty then we add it to the known
        else:
            # only saves received data only if it does not already exist
            if uniqueString not in saved_unique_identification:
                saved_face_encodings.append(first_unknown_face)
                saved_unique_identification.append(uniqueString)
                saved = True
            else:
                error = "You already submitted this request"


    # else if does not then
    else:
        error = "No Faces Found in Picture"

    # Return the result as json
    result = {
        "result": result,
        "saved": saved,
        "error": error,
    }
    return jsonify(result)