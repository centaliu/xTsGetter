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
  $sqlstr = "select * from tblfrmflv order by id limit 1";
  // run sql string and close connection
  $result = $conn->query($sqlstr);
  if ($result->num_rows > 0) {
      // output data of each row
      while($row = $result->fetch_assoc()) {
          echo $row["id"]."~".$row["WebURL"];
      }
  } else {
      echo "";
  }
  $conn->close();
}
?>