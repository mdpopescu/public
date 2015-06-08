__author__ = 'marcel'

import unittest

from Storage import Storage
from Formatter import Formatter
from App import App

class testAcceptance(unittest.TestCase):

    def setUp(self):
        storage = Storage()
        formatter = Formatter()
        self.sut = App(storage, formatter)

    def test_1(self):
        self.sut.post("Marcel", "Hello World!")
        self.sut.post("Marcel", "How is everyone?")

        lines = self.sut.get("Marcel")

        self.assertEquals(2, len(lines))
        self.assertEquals("How is everyone? (1 minute ago)", lines[0])
        self.assertEquals("Hello World! (2 minutes ago)", lines[1])

        self.sut.post("Anca", "We're doing great!")
        self.sut.follows("Marcel", "Anca")

        lines = self.sut.wall("Marcel")

        self.assertEquals(3, len(lines))
        self.assertEquals("Anca: We're doing great! (5 seconds ago)", lines[0])
        self.assertEquals("Marcel: How is everyone? (1 minute ago)", lines[1])
        self.assertEquals("Marcel: Hello World! (2 minutes ago)", lines[2])

if __name__ == '__main__':
    unittest.main()
