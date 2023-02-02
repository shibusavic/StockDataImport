# Development Journal

## 2023-01-28

### Progress

Been doing real-world testing of the process and making some changes along the way.

I finally resolved myself to the fact that virtually any field delivered by the eodhistoricaldata.com API can be null on any given call.
This meant a major database design overhaul with most of the fields being nullable.
Or course, that meant updating the data access objects.

Also, fields that should always be numbers might have blanks, nulls, or strings like "No Data."
So, lots of little tweaks to resolve those issues as they arose.

Q: How many API credits are spent executing the tests:
A: 1225; 93648 - 92423 = 1225; 2023-02-01