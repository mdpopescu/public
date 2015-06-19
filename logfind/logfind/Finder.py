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

        return [file_name for file_name in file_list if self.__content_matches(file_name, word_list, operation)]

    def __content_matches(self, file_name, word_list, operation):
        contents = self.fileSystem.load(file_name)
        return self.matcher.match(contents, word_list, operation)
