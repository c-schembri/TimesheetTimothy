\# Timesheet Timothy

Timothy requires a `jobs.txt` file to read from. This file is essentially the input data for the timesheet system. By default, Timothy looks in the current directory for the jobs file.
The structure of this file is as follows:

```
{
    "{day}": {
        "Entries": [
            { "JobCode": "{job_code}}", "Hours": "{hours}", "WorkType": "{work_type}" },   
        ]
    }
}
```

A complete example for the week:

```
{
	"Monday": {
		"Entries": [
			{ "JobCode": "BAMTM20001", "Hours": "4", "WorkType": "NOCHAR" },		
			{ "JobCode": "BAMTM20001", "Hours": "4", "WorkType": "NOCHAR" }
		]
	},
	
	"Tuesday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	},
	
	"Wednesday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	},
	
	"Thursday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	},
	
	"Friday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	}
}
```

\# Notes on Timothy's behaviour

You can enter multiple entries for the day, as seen in Monday on the above example, simply by entering a new anonymous object row.

Not every day is required to be defined (you may simply define a Wednesday day, for instance). However, if a day is defined, then `Entries` must be defined, with at least one entry. (There is no reason for a day to be defined if said day has no entries.)

For instance, the below is valid:

```
{
	"Wednesday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	}
}
```

However, the below is invalid:

```
{
	"Tuesday": {
	
	}
	
	"Wednesday": {
		"Entries": [	
			{ "JobCode": "BAMTM20001", "Hours": "8", "WorkType": "NOCHAR" }		
		]
	}
}
```

Currently, only five days of the week (i.e., Monday through Friday) are supported.

If you define a day twice, then the last one is used by Timothy (e.g., a Friday definition on line 10 will be used instead of a Friday definition on line 5).

For an entry, the minimal information needed is `JobCode` and `Hours`. `WorkType` is not required and Timothy will operate fine without `WorkType` being specified. (An exception will be thrown if `JobCode` or `Hours` is not defined for an entry.)

For instance, the below is valid:

```
{
	"Monday": {
		"Entries": [
			{ "JobCode": "BAMTM20001", "Hours": "4", "WorkType": "CHAR" },		
			{ "JobCode": "BAMTM20001", "Hours": "4", "WorkType": "NOCHAR" }
		]
	},
}
```
