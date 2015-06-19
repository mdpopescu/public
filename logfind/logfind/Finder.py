class Finder:

    def __init__(self, parser, fileSystem):
        self.parser = parser
        self.fileSystem = fileSystem

    def find(self, words, options=""):
        self.parser.parse_words(words)
        self.parser.parse_options(options)

        pattern = self.fileSystem.load(".logfind")
        file_list = self.fileSystem.get_files(pattern)
        print file_list
        for fileName in file_list:
            self.fileSystem.load(fileName)

        return []
