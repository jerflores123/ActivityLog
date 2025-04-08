Activity Log was the first application I worked on IJOS rewrite. 
EF core was new to me but I learned a lot from this example. 
It currently needs to be revised to match the rest of IJOS code process.

Currently it is taking a sql query and mapping the results to a DTO,
which worked up until we made the change to use ApplicationDBContext which changed the way we interacted with the database and wrote queries.
