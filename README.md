# HP5071Alogger
A logger application for diagnostic data of the Cs frequency standards 5071A
.

Self contained application for logging diagnostic data of primary Cs frequency standards of type 5071A. These instruments have been build by different manufactures (Hewlett Packard, Agilent, Keysight, Symmetricom, Microsemi).

## Usage

Edit the respective HP5071Alogger.exe.config file (XML format) with an editor of your choice. One can choose up to 5 different standards. Each of them are characterized by a name, the log file path and the serial communication port, respectively.

Logging data is stored as CSV (Comma Separated Values) text files. The actual delimiter can be set in the HP5071Alogger.exe.config file.


