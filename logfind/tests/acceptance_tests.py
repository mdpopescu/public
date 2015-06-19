from nose.tools import *
from logfind.Parser import Parser
from logfind.Finder import Finder
from logfind.FileSystem import FileSystem

class acceptance_tests:

    def __init__(self):
        parser = Parser()
        fileSystem = FileSystem()
        matcher = Matcher()
        self.sut = Finder(parser, fileSystem, matcher)

    @nottest
    def test_finds_test_log_with_and(self):
        files = self.sut.find("Lorem ipsum")

        assert_equal(1, len(files))
        assert_equal("S:\GIT-public\logfind\tests\test.log", files[0])

    @nottest
    def test_does_not_find_test_log_with_and(self):
        files = self.sut.find("Lorem gigi")

        assert_equal(0, len(files))

    @nottest
    def test_finds_test_log_with_or(self):
        files = self.sut.find("Lorem gigi", "-o")

        assert_equal(1, len(files))
        assert_equal("S:\GIT-public\logfind\tests\test.log", files[0])
