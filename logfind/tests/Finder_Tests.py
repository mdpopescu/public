from mock import MagicMock
from logfind.Finder import Finder

class finder_tests:

    def __init__(self):
        self.parser = MagicMock()
        self.fileSystem = MagicMock()
        self.sut = Finder(self.parser, self.fileSystem)

    def test_parses_the_words(self):
        self.sut.find("Lorem ipsum")

        self.parser.parse_words.assert_called_once_with("Lorem ipsum")

    def test_parses_the_options(self):
        self.sut.find("Lorem ipsum", "-o")

        self.parser.parse_options.assert_called_once_with("-o")

    def test_loads_the_logfind_file(self):
        self.sut.find("Lorem ipsum")

        self.fileSystem.load.assert_called_once_with(".logfind")

    def test_asks_for_names_of_files_matching_the_logfind_specifications(self):
        self.fileSystem.load.return_value = "*.log"

        self.sut.find("Lorem ipsum")

        self.fileSystem.get_files.assert_called_with("*.log")

    def test_loads_all_files_returned_by_get_files(self):
        self.fileSystem.load.return_value = "*.log"
        self.fileSystem.get_files.return_value = ["a.log", "b.log"]

        self.sut.find("Lorem ipsum")

        self.fileSystem.load.assert_any_call("a.log")
        self.fileSystem.load.assert_any_call("b.log")
