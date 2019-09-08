import face_recognition
import backup_restore

face_encodings_dictonary = {
    "male": {
        "saved_unique_identification": [],
        "saved_face_encodings": [],
    },
    "female": {
        "saved_unique_identification": [],
        "saved_face_encodings": [],
    }
}

# where face encodings get saved or loaded from if exists
folder_path = './temp/'
file_name = 'faces_encodings.npy'

def backup_face_encodings():
    backup_restore.backup(folder_path, file_name, face_encodings_dictonary)

def restore_face_encodings():
    loaded_data = backup_restore.restore(folder_path, file_name)
    global face_encodings_dictonary
    face_encodings_dictonary = loaded_data

# call detects faces method with respective data of female set or males set
def detect_faces_in_image(file_stream, uniqueString, gender, force_save):
    if gender in face_encodings_dictonary:
        saved_unique_identification = face_encodings_dictonary[gender]["saved_unique_identification"]
        saved_face_encodings = face_encodings_dictonary[gender]["saved_face_encodings"]
        return detect_faces(file_stream, uniqueString,saved_unique_identification, saved_face_encodings, force_save)
    else: return {
        "result": None,
        "saved": None,
        "error": "Gender is not valid",
    }
        

def detect_faces(file_stream, uniqueString, saved_unique_identification, saved_face_encodings, force_save): 

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

        if (force_save):
            if uniqueString not in saved_unique_identification:
                saved_face_encodings.append(first_unknown_face)
                saved_unique_identification.append(uniqueString)
                saved = True
                print(f'an Image of {uniqueString} added')
            return {
            "result": None,
            "saved": saved,
            "error": None,}


        # See if the first face in the uploaded image matches the known faces
        match_results = face_recognition.compare_faces(saved_face_encodings, first_unknown_face)

        ### Alpha
        if(not force_save):
            saved_face_encodings.append(first_unknown_face)
            saved_unique_identification.append(uniqueString)
            print(f'an Image of {uniqueString} added')
        ### Alpha

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
                sub_result.append(1 - distance[0])
                result.append(sub_result)

        # else if was empty then we add it to the known
        else:
            # only saves received data only if it does not already exist
            if uniqueString not in saved_unique_identification:
                saved_face_encodings.append(first_unknown_face)
                saved_unique_identification.append(uniqueString)
                saved = True
                print(f'an Image of {uniqueString} added')
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
    return result