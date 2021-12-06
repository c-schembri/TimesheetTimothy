# Timesheet Timothy

Timothy requires a `jobs.txt` file to read from. This file is essentially the input data for the timesheet system. By default, Timothy looks in the current directory for the jobs file.
The structure of this file is as follows:

`job-code` `hours`

For example:

```
BFTFP20005 8
BFTFP20005 8
BFTFP20005 8
BFTFP20005 4 ZZ16 4
ZZ16 8
```

The day entered into the timesheet is entered sequentially and is implied. That is, five rows of information (as seen in the above example) means that there are timesheets entered for five days (Monday through Friday, inclusive); three rows would be Monday, Tuesday, Wednesday, and so on.

As seen from the above example, everything is space delimited. Multiple job codes can be entered for the day simply by entering the `job-code` `hours` format multiple times on the same line, as seen on line four in the above example.
