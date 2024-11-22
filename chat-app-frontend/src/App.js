import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';

function App() {
    const [messages, setMessages] = useState([]);
    const [messageBody, setMessageBody] = useState('');
    const [chatId] = useState('f47ac10b-58cc-4372-a567-0e02b2c3d479'); // Hardcoded ChatId
    const [userId] = useState('3f2504e0-4f89-11d3-9a0c-0305e82c3301'); // Hardcoded UserId
    const [error, setError] = useState(null);
    const wsRef = useRef(null); // UseRef to persist WebSocket instance across renders

    // Fetch last 50 messages on component mount and initialize WebSocket
    useEffect(() => {
        console.log('Component mounted, initializing chat...');
        
        const initializeChat = async () => {
            try {
                await fetchLastMessages();
                connectWebSocket();
            } catch (err) {
                console.error('Error initializing chat:', err);
                setError('Failed to initialize chat. Please try again later.');
            }
        };

        initializeChat();

        return () => {
            if (wsRef.current) {
                wsRef.current.close();
                console.log('WebSocket connection closed on cleanup.');
            }
        };
    }, []); // Empty dependency array ensures this runs only once on mount

    // Log messages when updated
    useEffect(() => {
        if (messages.length > 2) {
            console.log('Last two messages:', messages.slice(-3));
        }
        console.log('Messages updated:', messages.length);
    }, [messages]);

    const fetchLastMessages = async () => {
        try {
            const response = await axios.get(`${process.env.REACT_APP_BACKEND_URL}/api/chat/${chatId}/messages`);
            setMessages(response.data.reverse());
        } catch (err) {
            console.error('Error fetching messages:', err);
            setError('Failed to fetch messages. Please check your connection.');
        }
    };

    const connectWebSocket = () => {
        try {
            // Append UserId and ChatId as query parameters in WebSocket URL
            const wsUrl = `${process.env.REACT_APP_CHAT_WS_URL || 'ws://localhost:5000/api/chat/ws'}?userId=${userId}&chatId=${chatId}`;
            console.log('WebSocket URL:', wsUrl);
            wsRef.current = new WebSocket(wsUrl);

            wsRef.current.onopen = () => console.log('WebSocket connection established.');

            wsRef.current.onmessage = (event) => {
                try {
                    console.log('WebSocket message received:', event.data);
                    const newMessage = JSON.parse(event.data); // Assuming the message is JSON
                    console.log('Parsed WebSocket message:', newMessage);
                    setMessages((prevMessages) => [...prevMessages, newMessage]);
                } catch (err) {
                    console.error('Error parsing WebSocket message:', err);
                }
            };

            wsRef.current.onerror = (err) => {
                console.error('WebSocket error:', err);
                setError('WebSocket connection error. Please refresh the page.');
            };

            wsRef.current.onclose = (event) => {
                if (event.code === 403) { // Handle access denial
                    setError('Access denied. You do not have permission to join this chat.');
                } else {
                    console.log('WebSocket connection closed.');
                }
            };
        } catch (err) {
            console.error('Error connecting to WebSocket:', err);
            setError('Failed to connect to WebSocket. Please check your connection.');
        }
    };

    const sendMessage = async () => {
        if (!messageBody.trim()) {
            setError('Message body cannot be empty.');
            return;
        }

        try {
            await axios.post(`${process.env.REACT_APP_BACKEND_URL}/api/chat/send`, {
                chatId,
                senderId: userId,
                body: messageBody,
            });
            setMessageBody('');
        } catch (err) {
            console.error('Error sending message:', err);
            setError('Failed to send message. Please try again.');
        }
    };

    return (
        <div>
            <h1>Chat</h1>

            {error && <div style={{ color: 'red' }}>{error}</div>}

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
                onKeyPress={(e) => {
                    if (e.key === 'Enter') {
                        sendMessage();
                    }
                }}
                placeholder="Type your message..."
            />

            <button onClick={sendMessage}>Send</button>
        </div>
    );
}

export default App;