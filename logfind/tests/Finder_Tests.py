from mock import Mock
from logfind.WordParser import WordParser
from logfind.OptionsParser import OptionsParser
from logfind.Finder import Finder
from logfind.FileSystem import FileSystem

class finder_tests:

    def __init__(self):
        self.wordParser = Mock(WordParser)
        self.optionsParser = Mock(OptionsParser)
        self.fileSystem = Mock(FileSystem)
        self.sut = Finder(self.wordParser, self.optionsParser, self.fileSystem)

    def test_parses_the_words(self):
        self.sut.find("Lorem ipsum")

        self.wordParser.parse.assert_called_once_with("Lorem ipsum")

    def test_parses_the_options(self):
        self.sut.find("Lorem ipsum", "-o")

        self.optionsParser.parse.assert_called_once_with("-o")

    def test_loads_the_logfind_file(self):
        self.sut.find("Lorem ipsum")

        self.fileSystem.load.assert_called_once_with(".logfind")

    def test_asks_for_names_of_files_matching_the_logfind_specifications(self):
        self.fileSystem.load.return_value = "*.log"

        self.sut.find("Lorem ipsum")

        self.fileSystem.get_files.assert_called_with("*.log")
