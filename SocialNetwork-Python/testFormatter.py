__author__ = 'marcel'

import unittest

import datetime

from Formatter import Formatter

class testFormatter(unittest.TestCase):
    def setUp(self):
        self.sut = Formatter()

    def test_returns_zero_seconds(self):
        result = self.sut.Format(datetime.timedelta(seconds = 0))

        self.assertEquals("0 seconds", result)

    def test_returns_one_second(self):
        result = self.sut.Format(datetime.timedelta(seconds = 1))

        self.assertEquals("1 second", result)

    def test_returns_many_seconds(self):
        result = self.sut.Format(datetime.timedelta(seconds = 25))

        self.assertEquals("25 seconds", result)

    def test_returns_one_minute(self):
        result = self.sut.Format(datetime.timedelta(minutes = 1))

        self.assertEquals("1 minute", result)

    def test_returns_many_minutes(self):
        result = self.sut.Format(datetime.timedelta(minutes = 15))

        self.assertEquals("15 minutes", result)
