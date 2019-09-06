# Facial Recognition Mini Server

Simple overview of use/purpose.

## Description

An flash server that makes using facial recognition easy with facial_recognition library.

## Getting Started

### Dependencies

* [dlib](https://gist.github.com/ageitgey/629d75c1baac34dfa5ca2a1928a7aeaf)
* [pipenv](https://github.com/pypa/pipenv)
* [flash](https://github.com/pallets/flask)
* [facial_recognition](https://github.com/ageitgey/face_recognition)

### Installing

* After you install pipenv 
* Just run `pipenv install` on the folder to install most of the dependencies
* if you face issues just go to the respective repo of each dependency

### Executing program

* How to use server
* Submit a `POST` request to http://host:port/ with the following form items
```
image, id, gender, force
```

## Acknowledgments

Inspiration, code snippets, etc.
* [flash example](https://github.com/ageitgey/face_recognition/blob/master/examples/web_service_example.py)
