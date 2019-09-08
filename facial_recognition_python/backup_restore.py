import numpy
from os import path,mkdir

access_rights = 0o755

# Save A dictonary to the path provided
def backup(folder_path, file_name, dictonary):
    relative_path_to_file = folder_path + file_name

    if not (path.exists(folder_path)):
        try:
            mkdir(folder_path, access_rights)
        except OSError:
            print(f"Failed to create Folder {folder_path}")
    
    if (path.exists(folder_path)):       
        numpy.save(relative_path_to_file, dictonary)
        print(f"Dictonary Backed up to {relative_path_to_file}")

def restore(folder_path, file_name):
    relative_path_to_file = folder_path + file_name

    if path.exists(folder_path) and path.isfile(relative_path_to_file):
        loaded_data = numpy.load(relative_path_to_file, None, True).item()
        print(f"Loaded Backup from {relative_path_to_file}")
        return loaded_data
    else:
        print(f'No Backup file found at path {relative_path_to_file} , starting fresh')
        return None
