import unittest

class AcceptanceTests(unittest.TestCase):

    def setUp(self):
        self.sut = App()

    def test_1(self):
        self.sut.Post("Marcel", "Hello World!")
        self.sut.Post("Marcel", "How is everyone?")

        lines = self.sut.Get("Marcel")

        self.assertEquals(len(lines), 2)
        self.assertEquals(lines[0], "How is everyone? (1 minute ago)")
        self.assertEquals(lines[1], "Hello World! (2 minutes ago)")

        self.sut.Post("Anca", "We're doing great!")
        self.sut.Follows("Marcel", "Anca")
