__author__ = 'marcel'

import unittest

import datetime

from Formatter import Formatter

class testFormatter(unittest.TestCase):
    def setUp(self):
        self.sut = Formatter()

    def test_returns_zero_seconds(self):
        result = self.sut.Format(datetime.time(0, 0, 0))

        self.assertEquals(result, "0 seconds")

    def test_returns_one_second(self):
        result = self.sut.Format(datetime.time(0, 0, 1))

        self.assertEquals(result, "1 second")

def suite():
    suite = unittest.TestSuite()

    suite.addTest(testFormatter("test_returns_zero_seconds"))
    suite.addTest(testFormatter("test_returns_one_second"))

    return suite
