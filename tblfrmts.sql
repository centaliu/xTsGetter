/* table creation */
CREATE TABLE tblfrmts (
    id int NOT NULL AUTO_INCREMENT,
    WebURL varchar(1024) DEFAULT '', /* direct URL of ts files */
		FlvFileName varchar(1024) DEFAULT '', /* filename of the remote flv file */
		Total int DEFAULT 0, /* how many ts file segments */
		OrinSrc varchar(1024) DEFAULT '', /* oringinal source URL of the flv file */
		CDateTime datetime DEFAULT CURRENT_TIMESTAMP, /* when this vocabulary is eastablished */
    PRIMARY KEY (id)
);

/* insert a row */
insert into tblfrmts (WebURL, FlvFileName, Total, OrinSrc, CDateTime) values ("http://www.url.com/", "xxx.flv", 200, "http://www.url.com/", CURRENT_TIMESTAMP);