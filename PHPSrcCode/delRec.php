<?php
$servername = "localhost";
$username = "id1051297_dbflv";
$password = $_GET["dbpwd"];
$dbname = "id1051297_dbflv";
if ($password == "") {
  echo "no password.";
} else {
  // Create connection
  $conn = new mysqli($servername, $username, $password, $dbname);
  // Check connection
  if ($conn->connect_error) die("ERROR");
  // assemble sql string
  $sqlstr = "delete from tblfrm".$_GET["tblname"]." where id = ".$_GET["id"];
  // run sql string and close connection
  $conn->query($sqlstr);
  $conn->close();
  // echo a string done
  echo "done";
}
?>