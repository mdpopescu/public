import fnmatch
import os

class FileSystem:

    def __init__(self):
        """ initialization """

    def load(self, fileName):
        with open(fileName, "r") as f:
            return f.read()

    def get_files(self, pattern):
        pattern = pattern.strip()

        matches = []
        for root, dirnames, filenames in os.walk("."):
            for filename in fnmatch.filter(filenames, pattern):
                matches.append(os.path.join(root, filename))

        return matches
