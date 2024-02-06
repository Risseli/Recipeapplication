const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const db = require('./db'); 

const app = express();
const port = 7003;

app.use(cors());
app.use(bodyParser.json());

//register
app.post('/register', async (req, res) => {
  const { email, username, password, nickname } = req.body;

  const checkExistingUserQuery = 'SELECT * FROM users WHERE email = ? OR username = ?';
  const checkExistingUserValues = [email, username];

  db.query(checkExistingUserQuery, checkExistingUserValues, (checkErr, checkResults) => {
    if (checkErr) {
      console.error('Error checking existing user:', checkErr);
      res.status(500).json({ success: false, message: 'Error checking existing user' });
    } else {
      if (checkResults.length > 0) {
        res.status(400).json({ success: false, message: 'Email or username already in use' });
      } else {
        const insertUserQuery = 'INSERT INTO users (email, username, password, nickname) VALUES (?, ?, ?, ?)';
        const insertUserValues = [email, username, password, nickname];

        db.query(insertUserQuery, insertUserValues, (insertErr, result) => {
          if (insertErr) {
            console.error('Error registering user:', insertErr);
            res.status(500).json({ success: false, message: 'Error registering user' });
          } else {
            res.json({ success: true, message: 'User registered successfully' });
          }
        });
      }
    }
  });
});

//login
app.post('/login', (req, res) => {
  const { username, password } = req.body;

  const sql = 'SELECT * FROM users WHERE username = ? AND password = ?';
  const values = [username, password];

  db.query(sql, values, (err, results) => {
    if (err) {
      console.error('Error during login:', err);
      res.status(500).json({ success: false, message: 'Error during login' });
    } else {
      if (results.length > 0) {
        res.json({ success: true, message: 'Login successful' });
      } else {
        res.status(401).json({ success: false, message: 'Invalid username or password' });
      }
    }
  });
});

app.listen(port, () => {
  console.log(`Server is running on port ${port}`);
});
