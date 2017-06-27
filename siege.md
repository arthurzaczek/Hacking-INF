#Siege commands

    siege -t20s -c100 https://hacking-inf.technikum-wien.at/api/Info/GetExamples?course=BIF-GPR2 

    siege -t20s -c100 --header="Authorization:Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJBcnRodXIgTWljaGFlbCBaYWN6ZWsiLCJ1bmlxdWVfbmFtZSI6InphY3pla yIsInJvbGUiOiJUZWFjaGVyIiwibmJmIjoxNDk4MTM4NDA0LCJleHAiOjE0OTkzNDgwMDQsImlhdCI6MTQ5ODEzODQwNCwiaXNzIjoiaHR0cHM6Ly9oYWNraW5nLWluZi50ZWNobmlrdW0td2llbi5hdCIsImF1ZCI6Imh0dHBzOi8v aGFja2luZy1pbmYudGVjaG5pa3VtLXdpZW4uYXQifQ.HvRIMDmfedIBDSJwRjGBUYSfYu4quqHt2DN0YvHaa0U"  https://hacking-inf.technikum-wien.at/api/Info/GetExamples?course=BIF-GPR2

> Note: Bearer Authorization Token is invalid now due to a change of the private JWT secret.

# Current

Hash: df2c808ebec5847cbf4744a71e3a47522c57f9e8

## Anonymous 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
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

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

# Improved User Handling 

Hash: 1e26a734e6dece3c81182040b5a82a9ee3af0aad

## Anonymous 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                   1439 hits
Availability:                 100.00 %
Elapsed time:                  19.28 secs
Data transferred:              17.55 MB
Response time:                  0.79 secs
Transaction rate:              74.64 trans/sec
Throughput:                     0.91 MB/sec
Concurrency:                   58.69
Successful transactions:        1439
Failed transactions:               0
Longest transaction:            1.32
Shortest transaction:           0.15
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Authenticated 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                     98 hits
Availability:                 100.00 %
Elapsed time:                  19.08 secs
Data transferred:               2.14 MB
Response time:                 10.39 secs
Transaction rate:               5.14 trans/sec
Throughput:                     0.11 MB/sec
Concurrency:                   53.37
Successful transactions:          98
Failed transactions:               0
Longest transaction:           17.38
Shortest transaction:           3.45
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# Reduce a select(1+n) to a select(1+1)

Hash: 41c5adcad9bc7fa718017390b1aad516d1f65616

## Anonymous 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                   1358 hits
Availability:                  99.93 %
Elapsed time:                  19.05 secs
Data transferred:              16.57 MB
Response time:                  0.88 secs
Transaction rate:              71.29 trans/sec
Throughput:                     0.87 MB/sec
Concurrency:                   63.08
Successful transactions:        1358
Failed transactions:               1
Longest transaction:            1.26
Shortest transaction:           0.21
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Authenticated 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                    653 hits
Availability:                  97.75 %
Elapsed time:                  19.72 secs
Data transferred:              14.38 MB
Response time:                  2.20 secs
Transaction rate:              33.11 trans/sec
Throughput:                     0.73 MB/sec
Concurrency:                   72.83
Successful transactions:         653
Failed transactions:              15
Longest transaction:           16.04
Shortest transaction:           0.50
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# Overall performance test

(Only authenticated)

    siege -t20s -c100 --header="Authorization:Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJBcnRodXIgTWljaGFlbCBaYWN6ZWsiLCJ1bmlxdWVfbmFtZSI6InphY3playIsInJvbGUiOiJUZWFjaGVyIiwibmJmIjoxNDk4MTM4NDA0LCJleHAiOjE0OTkzNDgwMDQsImlhdCI6MTQ5ODEzODQwNCwiaXNzIjoiaHR0cHM6Ly9oYWNraW5nLWluZi50ZWNobmlrdW0td2llbi5hdCIsImF1ZCI6Imh0dHBzOi8vaGFja2luZy1pbmYudGVjaG5pa3VtLXdpZW4uYXQifQ.HvRIMDmfedIBDSJwRjGBUYSfYu4quqHt2DN0YvHaa0U" -f hacking-inf-urls.txt

## Urls

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
https://hacking-inf.technikum-wien.at/api/Info/GetCourses
https://hacking-inf.technikum-wien.at/api/Info/GetExamples?course=BIF-GPR2
https://hacking-inf.technikum-wien.at/api/Info/GetCategories?course=BIF-GPR2
https://hacking-inf.technikum-wien.at/api/Info/GetCourse?name=BIF-GPR2
https://hacking-inf.technikum-wien.at/api/Info/GetExample?course=BIF-GPR2&name=ReadInt
https://hacking-inf.technikum-wien.at/api/Info/GetCompilerMessages
https://hacking-inf.technikum-wien.at/api/User?uid=zaczek
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Result

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                    911 hits
Availability:                 100.00 %
Elapsed time:                  19.26 secs
Data transferred:               4.94 MB
Response time:                  1.55 secs
Transaction rate:              47.30 trans/sec
Throughput:                     0.26 MB/sec
Concurrency:                   73.43
Successful transactions:         911
Failed transactions:               0
Longest transaction:            2.89
Shortest transaction:           0.25
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# Homepage

    siege -t20s -c100 https://hacking-inf.technikum-wien.at/

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
** SIEGE 3.0.8
** Preparing 100 concurrent users for battle.
The server is now under siege...
Lifting the server siege...      done.

Transactions:                   1509 hits
Availability:                 100.00 %
Elapsed time:                  19.51 secs
Data transferred:               1.08 MB
Response time:                  0.75 secs
Transaction rate:              77.34 trans/sec
Throughput:                     0.06 MB/sec
Concurrency:                   58.36
Successful transactions:        1509
Failed transactions:               0
Longest transaction:            1.12
Shortest transaction:           0.18
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Looks like, that 77 trans/sec is a limit. Maybe the limit of the mono `fast-cgi-server` and/or `nginx`

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  464 www-data  20   0 1209528 791980  43816 S 68,0 25,6   1:09.27 mono
26811 www-data  20   0   92172   6696   4560 R  9,0  0,2   1:17.30 nginx
26812 www-data  20   0   92236   6764   4560 R  9,0  0,2   1:15.40 nginx
26814 www-data  20   0   92424   6940   4560 R  8,0  0,2   1:17.79 nginx
26813 www-data  20   0   92172   6700   4560 R  6,0  0,2   1:10.92 nginx
    1 root      20   0   28600   3484   2244 S  0,0  0,1   0:17.64 systemd
    2 root      20   0       0      0      0 S  0,0  0,0   0:00.25 kthreadd
    3 root      20   0       0      0      0 R  0,0  0,0   0:48.60 ksoftirqd/0
    5 root       0 -20       0      0      0 S  0,0  0,0   0:00.00 kworker/0:0H
    7 root      20   0       0      0      0 R  0,0  0,0   3:37.40 rcu_sched
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
