import React, { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [messages, setMessages] = useState([]);
  const [messageBody, setMessageBody] = useState('');
  const [chatId] = useState('some-chat-id'); // Hardcoded for simplicity

  // Fetch last 50 messages on component mount
  useEffect(() => {
      fetchLastMessages();
      connectWebSocket();
  },);

  const fetchLastMessages = async () => {
    const response = await axios.get(`${process.env.REACT_APP_BACKEND_URL}/api/chat/${chatId}/messages`);
      setMessages(response.data);
  };

  const connectWebSocket = () => {
    const ws = new WebSocket(process.env.REACT_APP_CHAT_WS_URL || 'ws://localhost:5000/api/chat/ws');

      ws.onmessage = function(event) {
          const newMessage = event.data;
          setMessages(prevMessages => [...prevMessages, { body: newMessage }]);
      };

      return () => ws.close();
  };

  const sendMessage = async () => {
    await axios.post(`${process.env.REACT_APP_BACKEND_URL}/api/chat/send`, {
        chatId,
        senderId: 'some-sender-id', // Hardcoded for simplicity
        body: messageBody,
    });
      setMessageBody('');
  };

  return (
      <div>
          <h1>Chat</h1>
          <div>
              {messages.map((msg, index) => (
                  <div key={index}>
                      <strong>Anonymous</strong>: {msg.body}
                  </div>
              ))}
          </div>
          <input 
              type="text" 
              value={messageBody} 
              onChange={(e) => setMessageBody(e.target.value)} 
          />
          <button onClick={sendMessage}>Send</button>
      </div>
  );
}

export default App;