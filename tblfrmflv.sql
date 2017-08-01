/* table creation */
CREATE TABLE tblfrmflv (
    id int NOT NULL AUTO_INCREMENT,
    WebURL varchar(1024) DEFAULT '', /* URL of the post */
		CDateTime datetime DEFAULT CURRENT_TIMESTAMP, /* when the account is eastablished */
    PRIMARY KEY (id)
);

/* insert a row */
insert into tblfrmflv (WebURL, CDateTime) values ('http://www.url.com/video.php?id=701575', CURRENT_TIMESTAMP);