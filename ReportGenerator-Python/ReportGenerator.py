class ReportGenerator(object):

    def __init__(self, settings, dataReader, formatter, notifier):
        self.settings = settings
        self.dataReader = dataReader
        self.formatter = formatter
        self.notifier = notifier

    def Run(self, name):
        current = self.settings[name]

        data = self.dataReader.Read(current["query"])
        if (len(data.Rows)):
            report = self.formatter.Format(data)
            self.notifier.Send(current["sender"], current["recipients"], current["subject"], report)
