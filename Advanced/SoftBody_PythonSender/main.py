import socket
import time

# UDP settings
host = '127.0.0.1'  # Unity runs locally
port = 12345        # Must match Unity script port
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Example node positions
node_positions = [
    [0.0, 0.5, 0.0],
    [0.5, 0.5, 0.0],
    [1.0, 0.5, 0.0],
    [1.5, 0.5, 0.0],
    [2.0, 0.5, 0.0]
]

while True:
    # Simulate dynamic updates (e.g., move nodes in a sine wave)
    for i, node in enumerate(node_positions):
        node[1] = 0.5 + 0.02 * (i + 1) * time.time() % 0.2  # Update Y position dynamically

    # Serialize positions as "x,y,z;x,y,z;..."
    message = ";".join([",".join(map(str, pos)) for pos in node_positions])
    sock.sendto(message.encode(), (host, port))

    time.sleep(0.1)  # Send updates every 100ms
