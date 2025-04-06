# Neptune Protocol Specification

```
RFC: Neptune-1
Title: Neptune Protocol Specification
Author: Neptune Protocol Working Group
Status: Draft
Date: April 2025
```

## 1. Introduction

### 1.1 Purpose

This document specifies the Neptune Protocol, a lightweight, cross-channel messaging protocol designed for IRC-style communication across various transport layers including REST, UDP, and RADIO.

### 1.2 Scope

This specification defines:
- Message format and structure
- Authentication and security mechanisms
- Channel management
- Basic operation commands
- Transport layer abstraction

### 1.3 Terminology

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL NOT", "SHOULD", "SHOULD NOT", "RECOMMENDED", "MAY", and "OPTIONAL" in this document are to be interpreted as described in [RFC 2119](https://www.ietf.org/rfc/rfc2119.txt).

## 2. Protocol Overview

### 2.1 Design Goals

The Neptune Protocol aims to:
- Provide a unified messaging format across diverse transport layers
- Support end-to-end encryption for private messages
- Enable channel-based encryption for group communication
- Maintain compatibility with resource-constrained devices (e.g., ESP32)
- Support IRC-style channel operations
- Enable mesh networking capabilities

### 2.2 Protocol Stack

The Neptune Protocol operates on a layered model:

```
+-----------------------+
|     Application       |
|  (Client/Server Apps) |
+-----------------------+
|   Neptune Protocol    |
+-----------------------+
| Transport Abstraction |
+-----------------------+
|  REST | UDP | RADIO   |
+-----------------------+
```

## 3. Message Format

### 3.1 Basic Structure

All Neptune messages MUST be formatted as valid JSON objects with the following structure:

```json
{
  "version": "1.0",
  "header": {
    "messageId": "550e8400-e29b-41d4-a716-446655440000",
    "senderId": "squid@server.neptune.io",
    "timestamp": 1712409651,
    "type": "MESSAGE",
    "encryptionType": "E2E"
  },
  "routing": {
    "channel": "general",
    "recipient": "ed25519:3f1ebad5-2930-4228-9b8f-5d735a1c7f49",
    "transportMetadata": {
      "priority": "normal"
    }
  },
  "crypto": {
    "publicKey": "base64encodedkey...",
    "channelKeyId": "ch-key-2023-04-01",
    "iv": "base64encodedinit...",
    "signature": "base64encodedsignature..."
  },
  "payload": {
    "data": "encryptedBase64Data...",
    "format": "TEXT",
    "contentType": "text/plain"
  }
}
```

### 3.2 Message Components

#### 3.2.1 Version Field

The `version` field MUST contain a string representing the protocol version. The current version is "1.0".

#### 3.2.2 Header

The `header` object contains metadata about the message:

- `messageId`: A UUID v4 string that uniquely identifies the message
- `senderId`: The unique identifier of the sender in the format `localID@server.domain`
- `timestamp`: Unix timestamp in seconds
- `type`: Message type (see Section 3.3)
- `encryptionType`: Type of encryption used (see Section 3.4)

#### 3.2.3 Routing

The `routing` object contains information for message delivery:

- `channel`: Target channel name (REQUIRED for channel messages, OPTIONAL for private messages)
- `recipient`: Target recipient ID (REQUIRED for private messages, OPTIONAL for channel messages)
- `transportMetadata`: A map of key-value pairs containing transport-specific metadata

#### 3.2.4 Crypto

The `crypto` object contains cryptographic information:

- `publicKey`: The sender's public key (base64 encoded)
- `channelKeyId`: Identifier for the channel key used (for channel encryption)
- `iv`: Initialization vector for encryption (base64 encoded)
- `signature`: Digital signature of the message (base64 encoded)

#### 3.2.5 Payload

The `payload` object contains the message content:

- `data`: The encrypted message content (base64 encoded)
- `format`: The format of the decrypted content (TEXT, JSON, BINARY)
- `contentType`: MIME type of the content

### 3.3 Message Types

The following message types are defined:

| Type    | Description                          |
|---------|--------------------------------------|
| MESSAGE | Regular message to a channel         |
| PRIVMSG | Private message to a specific user   |
| JOIN    | Request to join a channel            |
| LEAVE   | Request to leave a channel           |
| PING    | Connection check                     |
| PONG    | Response to a ping                   |
| INFO    | Channel or server information        |
| ERROR   | Error notification                   |

### 3.4 Encryption Types

The following encryption types are defined:

| Type    | Description                           |
|---------|---------------------------------------|
| E2E     | End-to-end encryption                 |
| CHANNEL | Channel encryption with shared key    |
| NONE    | No encryption (not recommended)       |

## 4. Security

### 4.1 Identity and Authentication

Each participant in the Neptune protocol MUST be identified by a unique ID. The ID format MUST support federation across different servers.

#### 4.1.1 User Identity Format

The complete identity format is:

```
localID@server.domain
```

Where:
- `localID` is a local identifier on the specific server
- `server.domain` is the Neptune server domain

For example:
```
squid@server.neptune.io
```

#### 4.1.2 Cryptographic Identity

Each user's `localID` SHOULD be derived from their public key. The recommended format is:

```
algorithm:public-key-hash
```

For complete federated identity, the full format would be:

```
algorithm:public-key-hash@server.domain
```

For example:
```
ed25519:74dc82e3@server.neptune.io
```

For optimal usability, servers MAY allow users to register friendly usernames that map to their cryptographic identities.

### 4.2 End-to-End Encryption

For private messages (PRIVMSG), end-to-end encryption MUST be used by default:

1. The sender encrypts the message using the recipient's public key
2. Only the recipient can decrypt the message using their private key
3. The `crypto.publicKey` field MUST contain the sender's public key
4. The `crypto.iv` field MUST contain a unique initialization vector

### 4.3 Channel Encryption

For channel messages (MESSAGE), channel encryption SHOULD be used:

1. Each channel has a symmetric encryption key shared among members
2. The key is identified by a unique ID in the `crypto.channelKeyId` field
3. Channel keys SHOULD be rotated periodically
4. New members receive the current channel key upon joining

### 4.4 Message Signatures

All messages MUST be digitally signed by the sender:

1. The signature covers the entire message except the `crypto.signature` field
2. The signature is computed using the sender's private key
3. Recipients validate the signature using the sender's public key
4. Messages with invalid signatures MUST be rejected

## 5. Channel Operations

### 5.1 Channel Names

Channel names MUST:
- Begin with a "#" character
- Contain only alphanumeric characters, hyphens, and underscores
- Be between 2 and 32 characters in length (including the "#")

### 5.2 Joining Channels

To join a channel, a client sends a message with:
- `header.type`: "JOIN"
- `routing.channel`: Target channel name

The server responds with channel information and current members.

### 5.3 Leaving Channels

To leave a channel, a client sends a message with:
- `header.type`: "LEAVE"
- `routing.channel`: Target channel name

### 5.4 Channel Messages

To send a message to a channel, a client sends a message with:
- `header.type`: "MESSAGE"
- `routing.channel`: Target channel name
- `payload.data`: The encrypted message content

## 6. Private Messages

To send a private message, a client sends a message with:
- `header.type`: "PRIVMSG"
- `routing.recipient`: The recipient's unique ID
- `payload.data`: The encrypted message content

## 7. Federation

### 7.1 Server-to-Server Communication

Servers in the Neptune network MUST be able to communicate with each other to route messages between users on different servers.

#### 7.1.1 Server Discovery

Servers SHOULD support DNS-based discovery using SRV records:

```
_neptune._tcp.server.domain SRV 10 0 1837 neptune.server.domain
```

#### 7.1.2 Server Authentication

Servers MUST authenticate each other using TLS with certificate validation.

#### 7.1.3 Message Routing

When a message is addressed to a recipient on another server:

1. The originating server identifies the destination server from the recipient's ID (`user@server.domain`)
2. It establishes a secure connection to the destination server if one doesn't exist
3. It forwards the message to the destination server
4. The destination server delivers the message to the local recipient

#### 7.1.4 Channel Federation

Channels can be federated across servers:

1. Global channels MUST be prefixed with "##" (e.g., "##general")
2. Server-specific channels MUST be prefixed with "#" (e.g., "#general")
3. Global channels are replicated across all participating servers

## 8. Implementation Considerations

### 8.1 Transport Layer Abstraction

Implementations SHOULD provide transport layer abstraction that handles:

1. Message serialization and deserialization
2. Transport-specific routing
3. Reconnection logic
4. Error handling

### 8.2 Resource-Constrained Devices

For resource-constrained devices (e.g., ESP32):

1. A lightweight implementation MAY omit certain fields
2. Required fields are:
   - `version`
   - `header.messageId`
   - `header.senderId`
   - `header.type`
   - `routing.*` (relevant fields)
   - `payload.data`

### 8.3 Performance Considerations

1. Messages SHOULD be kept under 8KB for optimal performance
2. For larger data, implementations SHOULD use chunking
3. Binary data SHOULD be base64 encoded in the payload

## 9. References

1. RFC 2119 - Key words for use in RFCs to Indicate Requirement Levels
2. RFC 7519 - JSON Web Token (JWT)
3. RFC 8259 - The JavaScript Object Notation (JSON) Data Interchange Format

## Appendix A: Examples

### A.1 Channel Message

```json
{
  "version": "1.0",
  "header": {
    "messageId": "550e8400-e29b-41d4-a716-446655440000",
    "senderId": "squid@server.neptune.io",
    "timestamp": 1712409651,
    "type": "MESSAGE",
    "encryptionType": "CHANNEL"
  },
  "routing": {
    "channel": "#general",
    "transportMetadata": {
      "priority": "normal"
    }
  },
  "crypto": {
    "publicKey": "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=",
    "channelKeyId": "ch-key-2023-04-01",
    "iv": "MTIzNDU2Nzg5MDEyMzQ1Ng==",
    "signature": "c2lnbmF0dXJlLWRhdGEtaGVyZQ=="
  },
  "payload": {
    "data": "U29tZSBlbmNyeXB0ZWQgbWVzc2FnZSBjb250ZW50",
    "format": "TEXT",
    "contentType": "text/plain"
  }
}
```

### A.2 Private Message

```json
{
  "version": "1.0",
  "header": {
    "messageId": "550e8400-e29b-41d4-a716-446655440001",
    "senderId": "squid@server.neptune.io",
    "timestamp": 1712409653,
    "type": "PRIVMSG",
    "encryptionType": "E2E"
  },
  "routing": {
    "recipient": "octopus@other.neptune.io",
    "transportMetadata": {}
  },
  "crypto": {
    "publicKey": "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=",
    "iv": "MTIzNDU2Nzg5MDEyMzQ1Ng==",
    "signature": "c2lnbmF0dXJlLWRhdGEtaGVyZQ=="
  },
  "payload": {
    "data": "U29tZSBlbmNyeXB0ZWQgbWVzc2FnZSBjb250ZW50",
    "format": "TEXT",
    "contentType": "text/plain"
  }
}
```

### A.3 Join Channel

```json
{
  "version": "1.0",
  "header": {
    "messageId": "550e8400-e29b-41d4-a716-446655440002",
    "senderId": "squid@server.neptune.io",
    "timestamp": 1712409655,
    "type": "JOIN",
    "encryptionType": "NONE"
  },
  "routing": {
    "channel": "#general",
    "transportMetadata": {}
  },
  "crypto": {
    "publicKey": "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=",
    "signature": "c2lnbmF0dXJlLWRhdGEtaGVyZQ=="
  },
  "payload": {
    "data": "",
    "format": "TEXT",
    "contentType": "text/plain"
  }
}
```

## Appendix B: Recommended Cryptographic Algorithms

| Purpose | Recommended Algorithm | Alternative |
|---------|------------------------|------------|
| Key Pair Generation | Ed25519 | ECDSA P-256 |
| Symmetric Encryption | AES-256-GCM | ChaCha20-Poly1305 |
| Key Derivation | HKDF-SHA-256 | PBKDF2 |
| Digital Signatures | Ed25519 | ECDSA P-256 |
| Hashing | SHA-256 | BLAKE2b |
