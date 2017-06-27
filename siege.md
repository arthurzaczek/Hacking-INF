# Current

## Anonymous 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
root@lab-inf:~# siege -t20s -c100 https://hacking-inf.technikum-wien.at/api/Info/GetExamples?course=BIF-GPR2     ** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                   1364 hits
Availability:                 100.00 %
Elapsed time:                  19.88 secs
Data transferred:              16.63 MB
Response time:                  0.93 secs
Transaction rate:              68.61 trans/sec
Throughput:                     0.84 MB/sec
Concurrency:                   63.49
Successful transactions:        1364
Failed transactions:               0
Longest transaction:            2.72
Shortest transaction:           0.21
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Authenticated

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~root@lab-inf:~# siege -t20s -c100 --header="Authorization:Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJBcnRodXIgTWljaGFlbCBaYWN6ZWsiLCJ1bmlxdWVfbmFtZSI6InphY3pla yIsInJvbGUiOiJUZWFjaGVyIiwibmJmIjoxNDk4MTM4NDA0LCJleHAiOjE0OTkzNDgwMDQsImlhdCI6MTQ5ODEzODQwNCwiaXNzIjoiaHR0cHM6Ly9oYWNraW5nLWluZi50ZWNobmlrdW0td2llbi5hdCIsImF1ZCI6Imh0dHBzOi8v aGFja2luZy1pbmYudGVjaG5pa3VtLXdpZW4uYXQifQ.HvRIMDmfedIBDSJwRjGBUYSfYu4quqHt2DN0YvHaa0U"  https://hacking-inf.technikum-wien.at/api/Info/GetExamples?course=BIF-GPR2
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                     80 hits
Availability:                 100.00 %
Elapsed time:                  19.85 secs
Data transferred:               1.74 MB
Response time:                 10.56 secs
Transaction rate:               4.03 trans/sec
Throughput:                     0.09 MB/sec
Concurrency:                   42.56
Successful transactions:          80
Failed transactions:               0
Longest transaction:           18.59
Shortest transaction:           3.65
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# Improved User Handling ()
