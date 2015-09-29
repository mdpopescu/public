import unittest
from mock import MagicMock

from ReportGenerator import ReportGenerator

class ReportGeneratorTests(unittest.TestCase):

    def setUp(self):
        self.settings = {
            "sample": {
                "query": "SELECT * FROM data",
                "sender": "report_sender@baml.com",
                "recipients": ["report_target@baml.com"],
                "subject": "Reports for 2000-01-02"
            }
        }
        self.dataReader = MagicMock()
        self.formatter = MagicMock()
        self.notifier = MagicMock()

        self.sut = ReportGenerator(self.settings, self.dataReader, self.formatter, self.notifier)

    def test_RunReadsFromTheDatabase(self):
        self.sut.Run("sample")

        self.dataReader.Read.assert_called_with("SELECT * FROM data")

    def test_RunThrowsIfReportUnknown(self):
        self.assertRaises(Exception, self.sut.Run, "unknown")

    def test_RunFormatsTheData(self):
        table = MagicMock()
        table.Columns = ["a", "b", "c"]
        table.Rows = [
            [1, "x", "stuff"],
            [2, "y", "more stuff"]
        ]
        self.dataReader.Read.return_value = table

        self.sut.Run("sample")

        self.formatter.Format.assert_called_with(table)

    def test_RunDoesNotFormatTheDataIfNoRows(self):
        table = MagicMock()
        table.Columns = ["a", "b", "c"]
        table.Rows = []
        self.dataReader.Read.return_value = table

        self.sut.Run("sample")

        self.assertEquals(0, self.formatter.Format.call_count)

    def test_RunSendsNotification(self):
        table = MagicMock()
        table.Columns = ["a", "b", "c"]
        table.Rows = [
            [1, "x", "stuff"],
            [2, "y", "more stuff"]
        ]
        self.dataReader.Read.return_value = table
        self.formatter.Format.return_value = "abc"

        self.sut.Run("sample")

        self.notifier.Send.assert_called_with("report_sender@baml.com", ["report_target@baml.com"], "Reports for 2000-01-02", "abc")

    def test_RunDoesNotSendNotificationIfNoRows(self):
        table = MagicMock()
        table.Columns = ["a", "b", "c"]
        table.Rows = []
        self.dataReader.Read.return_value = table
        self.formatter.Format.return_value = "abc"

        self.sut.Run("sample")

        self.assertEquals(0, self.notifier.Send.call_count)
