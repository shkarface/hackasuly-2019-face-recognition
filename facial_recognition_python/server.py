from face_recognizer import detect_faces_in_image, backup_face_encodings, restore_face_encodings
from flask import Flask, jsonify, request, redirect
from signal import SIGABRT, SIGILL, SIGINT, SIGSEGV, SIGTERM, signal
import sys


# Port

port = 5000
host = "localhost"

custom_port = input("Server port? Default is 5000\n")
custom_host = input("Server host? Default is localhost\n")
user_choice_to_restore = input("""Do you want to restore old data?
Y for Yes or anything else for No
Back must exist in the following directory relative to this server
./temp/faces_encodings.npy
Warning if You do not restore, auto backup will replace old one\n""")

if (custom_port is not None and custom_port != ''):
    port = int(custom_port)

if (custom_host is not None and custom_host != ''):
    host = custom_host

if (user_choice_to_restore is not None and user_choice_to_restore != ''):
    first_letter = user_choice_to_restore[0]
    if (first_letter == 'Y' or first_letter == 'y'):
        restore_face_encodings()

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
        force_save = request.form['force']

        if force_save == 0 or force_save[0] == '0':
            force_save = False
        elif force_save == 1 or force_save[0] == '1':
            force_save = True
        
        if file.filename == '':
            return redirect(request.url)

        if file and allowed_file(file.filename) and unique_string is not None and gender is not None:
            # The image file seems valid! Detect faces and return the result.
            return jsonify(detect_faces_in_image(file, unique_string, gender, force_save))

    # If no valid image file was uploaded, show the file upload form:
    
    return '''
    <!doctype html>
    <title>Recognize Faces?</title>
    <h1>Upload a picture and see if it get recognized!</h1>
    <form method="POST" enctype="multipart/form-data">
      <label>File</label><input type="file" name="file"></br>
      <label>String</label><input type="text" name="string"></br>
      <p>Please select your gender:</p> </br>
        <input type="radio" name="gender" value="male"> Male<br>
        <input type="radio" name="gender" value="female"> Female<br>
        <p>Force Save?:</p> </br>
         <input type="radio" name="force" value=1> Yes <br>
        <input type="radio" name="force" value=0> No <br>      
      <input type="submit" value="Upload"></br>
    </form>
    '''

# Runs before the server exits
# Excluding SIGBREAK signal
def run_before_exit(*args):
    backup_face_encodings()
    sys.exit(0)

for sigcode in (SIGABRT, SIGILL, SIGINT, SIGSEGV, SIGTERM):
    signal(sigcode, run_before_exit)


if __name__ == "__main__":
    app.run(host, port, debug=True, use_reloader=False)