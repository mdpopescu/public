__author__ = 'marcel'

import unittest

from App import App

class testAcceptance(unittest.TestCase):

    def setUp(self):
        self.sut = App(None)

    @unittest.expectedFailure
    def test_1(self):
        self.sut.post("Marcel", "Hello World!")
        self.sut.post("Marcel", "How is everyone?")

        lines = self.sut.get("Marcel")

        self.assertEquals(len(lines), 2)
        self.assertEquals(lines[0], "How is everyone? (1 minute ago)")
        self.assertEquals(lines[1], "Hello World! (2 minutes ago)")

        self.sut.post("Anca", "We're doing great!")
        self.sut.follows("Marcel", "Anca")

        lines = self.sut.wall("Marcel")

        self.assertEquals(len(lines), 3)
        self.assertEquals(lines[0], "Anca: We're doing great! (5 seconds ago)")
        self.assertEquals(lines[1], "Marcel: How is everyone? (1 minute ago)")
        self.assertEquals(lines[2], "Marcel: Hello World! (2 minutes ago)")

if __name__ == '__main__':
    unittest.main()
