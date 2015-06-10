__author__ = 'marcel'

import unittest

import datetime

from Storage import Storage
from Users import Users
from Formatter import Formatter
from Clock import Clock
from App import App

class testAcceptance(unittest.TestCase):

    def setUp(self):
        storage = Storage()
        users = Users()
        formatter = Formatter()
        self.sut = App(storage, users, formatter)

    @unittest.skip("skip")
    def test_1(self):
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 4, 5))
        self.sut.post("Marcel", "Hello World!")
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 5, 5))
        self.sut.post("Marcel", "How is everyone?")

        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 6, 5))
        lines = self.sut.get("Marcel")

        self.assertEquals(2, len(lines))
        self.assertEquals("How is everyone? (1 minute ago)", lines[0])
        self.assertEquals("Hello World! (2 minutes ago)", lines[1])

        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 6, 5))
        self.sut.post("Anca", "We're doing great!")
        self.sut.follows("Marcel", "Anca")

        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 6, 10))
        lines = self.sut.wall("Marcel")

        self.assertEquals(3, len(lines))
        self.assertEquals("Anca: We're doing great! (5 seconds ago)", lines[0])
        self.assertEquals("Marcel: How is everyone? (1 minute ago)", lines[1])
        self.assertEquals("Marcel: Hello World! (2 minutes ago)", lines[2])

if __name__ == '__main__':
    unittest.main()
