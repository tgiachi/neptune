# Neptune Protocol Specification

## RFC-NPT-0001: Neptune Decentralized Messaging Protocol (Draft)

**Version:** 0.1.0
**Status:** Draft
**Author:** squid@squid@stormwind.it
**Date:** 2025-04-07

---

## 1. Abstract

Neptune is a decentralized, delay-tolerant messaging protocol designed for secure, asynchronous communication in environments with limited or no Internet access. Messages are end-to-end encrypted and transmitted via mobile hardware nodes (e.g., ESP32 devices) using short-range radio protocols. Messages propagate through a store-and-forward mesh until reaching their destination or a connected exit node.

---

## 2. Terminology

- **Vector Node**: A mobile device (usually ESP32-based) that stores and forwards messages across the network.
- **Exit Node**: A stationary node connected to the Internet. Acts as a gateway for REST-based delivery and user registration.
- **Message**: An encrypted payload with metadata, transferred through Neptune.
- **Beacon**: A periodic radio broadcast sent by vector nodes to announce their availability.
- **History**: A compact string describing the route the message has taken through the network (device IDs and GPS coordinates).

---

## 3. Node Roles

### 3.1 Vector Node

- Emits beacons regularly over radio (e.g., ESP-NOW, LoRa).
- Detects nearby nodes and engages in message exchange.
- Stores messages in internal memory or SD card.
- Forwards messages based on hop limit and recipient match.
- Appends its `deviceId` and GPS coordinates to the `history`.

### 3.2 Exit Node

- Hosts a REST API for clients and federated servers.
- Interfaces with a local vector node over serial/Bluetooth/USB.
- Acts as a bridge between the Neptune mesh and the Internet.
- Registers users and serves public keys.

---

## 4. Message Format

Neptune messages follow a compact CBOR format for transmission and may be converted to JSON on the server side.

```json
{
  "id": "uuid-v4",
  "from": "squid@stormwind.it",
  "to": "octopus@deeprelay.org",
  "timestamp": 1712500000,
  "payload": "base64(encrypted_payload)",
  "signature": "base64(sender_signature)",
  "hops": 2,
  "maxHops": 5,
  "history": "nodeA:43.7696,11.2558;nodeB:43.7700,11.2560"
}
```

**Field Notes:**

- `id`: Unique identifier (UUID).
- `from`, `to`: Email-like user identifiers.
- `timestamp`: Unix epoch timestamp at creation time.
- `payload`: Encrypted content, only decryptable by the receiver.
- `signature`: Digital signature of the sender over the payload.
- `hops`: Number of relay points the message has traversed.
- `maxHops`: Time-to-live limit to prevent infinite propagation.
- `history`: Compact route log for audit and debug, stored as a single string.

---

## 5. Message Lifecycle

### 5.1 Discovery

- Vector nodes emit periodic beacons.
- Nodes receiving the beacon respond to initiate message exchange.

### 5.2 Handshake

- Handshake involves message ID exchange.
- Each node requests only messages it does not yet possess.

### 5.3 Forwarding

- Upon transfer, the node:
  - Appends itself to the `history` string.
  - Increments `hops`.
- If `hops >= maxHops`, the message is discarded.

---

## 6. Cryptography

- Encryption: End-to-end using recipient’s public key (e.g., RSA or ECC).
- Signing: Messages are signed using the sender’s private key.
- Public Key Retrieval: Via REST API endpoint exposed by the sender's server:

      GET /users/squid@stormwind.it/key

---

## 7. Federation

- Users are registered on servers with email-like identifiers.
- Messages destined to other servers are delivered over REST:

      POST https://deeprelay.org/messages

- If the destination server is unreachable, the message is stored and routed via the mesh.

---

## 8. Optimizations & Future Work

- Chunked payload support for large messages.
- Group messaging and multicast.
- Binary compression (e.g., CBOR + Deflate).
- Identity verification via Web of Trust.
- Optional message expiration or geofencing metadata.
- Adaptive routing heuristics in the mesh.

---

## 9. License

This specification is open and free to implement under the MIT License or any compatible permissive license.
