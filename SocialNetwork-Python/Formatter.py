__author__ = 'marcel'

class Formatter:
    def Format(self, duration):
        if duration.seconds >= 60:
            unit = "minute"
            value = duration.seconds // 60
        else:
            unit = "second"
            value = duration.seconds

        suffix = "" if value == 1 else "s"

        return str(value) + " " + unit + suffix
