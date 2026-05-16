 > [!WARNING]
> ## This project is deprecated and no longer maintained
>
> **Virtual Data Warehouse** has been superseded by [Agnostic Data Labs](https://www.agnosticdatalabs.com), a free (closed-source) successor product.
>
> This repository is retained for historical reference only. No further updates or bug fixes will be released, and no pull requests or issues will be accepted. The repository will be archived shortly.

---

# Virtual Data Warehouse

The Virtual Data Warehouse (VDW) and the associated metadata management software TEAM are free tools to quickly prototype your Data Warehouse model (output), validate your metadata and generally get insight in the your Data Warehouse patterns.

VDW can be used to test out various new patterns and ideas, as well as regression testing for pattern changes in various technologies and platforms..

The key concepts are related to the idea that, if you can generate all your ETL and you can ‘replay’ your entire Data Warehouse (when you refactor for instance – using Persistent Staging Areas), you can basically virtualise your entire Data Warehouse (I coined the term ‘NoETL’ for this :-)).

Your Data Warehouse model essentially becomes a ‘schema-on-read’ on your raw data.

More information at present is here: http://roelantvos.com/blog/articles-and-white-papers/virtualisation-software/.

# A library of patterns

VDW comes with a library of patterns based on Handlebars as templating engine. With this it is possible to generate a sample Data Warehouse using Data Vault methodology.
