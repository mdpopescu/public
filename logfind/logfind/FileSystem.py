from glob import glob

class FileSystem:

    def __init__(self):
        """ initialization """

    def load(self, fileName):
        with open(fileName, "r") as f:
            return f.read()

    def get_files(self, pattern):
        return glob(pattern)
