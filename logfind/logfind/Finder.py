class Finder:

    def __init__(self, parser, fileSystem, matcher):
        self.parser = parser
        self.fileSystem = fileSystem
        self.matcher = matcher

    def find(self, words, options=""):
        word_list = self.parser.parse_words(words)
        operation = self.parser.parse_options(options)

        pattern = self.fileSystem.load(".logfind")
        file_list = self.fileSystem.get_files(pattern)

        result = []
        for fileName in file_list:
            contents = self.fileSystem.load(fileName)
            if self.matcher.match(contents, word_list, operation):
                result.append(fileName)

        return result
