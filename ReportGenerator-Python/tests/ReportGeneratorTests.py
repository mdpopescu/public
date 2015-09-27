import unittest
from mock import MagicMock

from ReportGenerator import ReportGenerator

class ReportGeneratorTests(unittest.TestCase):

    def setUp(self):
        self.settings = {
            "sample": {
                "query": "SELECT * FROM data"
            }
        }
        self.dataReader = MagicMock()

        self.sut = ReportGenerator(self.settings, self.dataReader)

    def test_RunReadsFromTheDatabase(self):
        self.sut.Run("sample")

        self.dataReader.Read.assert_called_with("SELECT * FROM data")

    def test_RunThrowsIfReportUnknown(self):
        self.assertRaises(Exception, self.sut.Run, "unknown")
