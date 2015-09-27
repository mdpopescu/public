import unittest
from mock import MagicMock

from ReportGenerator import ReportGenerator

class AcceptanceTests(unittest.TestCase):

    def setUp(self):
        self.settings = {
            "sample": {
                "query": "SELECT * FROM data"
            }
        }

        table = MagicMock()
        table.Columns = ['a', 'b', 'c']
        table.Rows = [
            [1, 'x', 'stuff'],
            [2, 'y', 'more stuff']
        ]

        self.dataReader = MagicMock()
        self.dataReader.Read.return_value=table

        self.notifier = MagicMock()

        self.sut = ReportGenerator(self.settings, self.dataReader)

        # overwrite the notifier - I don't want to actually send an email
        self.sut.notifier = self.notifier

    def test_run(self):
        expected = """<html>
        <body>
            <table>
                <thead>
                    <th>a</th>
                    <th>b</th>
                    <th>c</th>
                </thead>
                <tbody>
                    <tr>
                        <td>1</td>
                        <td>x</td>
                        <td>stuff</td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>y</td>
                        <td>more stuff</td>
                    </tr>
                </tbody>
            </table>
        </body>
        </html>
        """

        self.sut.Run("sample")

        self.notifier.Notify.assert_called_with('report_sender@baml.com', ['report_target@baml.com'], 'Reports for 2000-01-02', expected)
