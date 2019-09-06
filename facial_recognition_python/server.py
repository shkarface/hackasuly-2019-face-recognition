from face_recognizer import detect_faces_in_image
from flask import Flask, jsonify, request, redirect

# Port

port = 5000
host = "localhost"

# You can change this to any folder on your system
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif'}

app = Flask(__name__)


def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS


@app.route('/', methods=['GET', 'POST'])
def upload_image():
    # Check if a valid image file was uploaded
    if request.method == 'POST':
        if 'file' not in request.files:
            return redirect(request.url)

        file = request.files['file']
        unique_string = request.form['string']
        gender = request.form['gender']

        if file.filename == '':
            return redirect(request.url)

        if file and allowed_file(file.filename) and unique_string is not None and gender is not None:
            # The image file seems valid! Detect faces and return the result.
            return jsonify(detect_faces_in_image(file, unique_string, gender))

    # If no valid image file was uploaded, show the file upload form:
    
    return '''
    <!doctype html>
    <title>Recognize Faces?</title>
    <h1>Upload a picture and see if it get recognized!</h1>
    <form method="POST" enctype="multipart/form-data">
      <label>File</label><input type="file" name="file"></br>
      <label>String</label><input type="text" name="string"></br>
      <label>Gender</label><input type="text" name="gender"></br>
      <input type="submit" value="Upload"></br>
    </form>
    '''




if __name__ == "__main__":
    app.run(host, port, debug=True)