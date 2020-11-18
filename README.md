# Virtual Data Warehouse

The Virtual Data Warehouse (VDW) and the associated metadata management software TEAM are free tools to quickly prototype your Data Warehouse model (output), validate your metadata and generally get insight in the your Data Warehouse patterns.

VDW can be used to test out various new patterns and ideas, as well as regression testing for pattern changes in various technologies and platforms..

The key concepts are related to the idea that, if you can generate all your ETL and you can ‘replay’ your entire Data Warehouse (when you refactor for instance – using Persistent Staging Areas), you can basically virtualise your entire Data Warehouse (I coined the term ‘NoETL’ for this :-)).

Your Data Warehouse model essentially becomes a ‘schema-on-read’ on your raw data.

More information at present is here: http://roelantvos.com/blog/articles-and-white-papers/virtualisation-software/.
