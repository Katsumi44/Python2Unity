import socket
import json
import time
import math

# Step 1: Set parameters for the sinusoidal motion
amplitude = 5.0  # The maximum displacement of the nodes from their starting point
frequency = 1.0  # The frequency of the oscillation (how fast they swing)
phase_shift = [0, 1, 2, 3, 4]  # Phase shift for each node to stagger their motion
center_position = [10.0, 10.0, 10.0]  # The starting point for all nodes


# Step 2: Define a function to simulate the swinging trajectory
def simulate_swinging_motion(time_step):
    """Simulate the positions of the 5 points in a sinusoidal motion."""
    points = []
    for i in range(5):
        # Use a sine function to simulate the swinging motion
        x = center_position[0] + amplitude * math.sin(frequency * time_step + phase_shift[i])
        y = center_position[1]  # Keep the y-coordinate constant
        z = center_position[2] + amplitude * math.cos(frequency * time_step + phase_shift[i])
        points.append({"x": x, "y": y, "z": z})
    return points


# Step 3: Define a function to format the data as JSON
def format_data(points):
    """Format the data as a JSON string."""
    return json.dumps({"points": points})


# Step 4: Set up a UDP socket
HOST = "127.0.0.1"  # Localhost (to send data to Unity running on the same machine)
PORT = 5005  # Port number to match Unity's listener


# Step 5: Send data periodically
def send_data():
    """Send simulated swinging data at regular intervals."""
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as sock:
        time_step = 0  # Start time at 0
        while True:
            # Simulate the positions of the points
            points = simulate_swinging_motion(time_step)

            # Format the data
            data = format_data(points)

            # Send the data
            sock.sendto(data.encode(), (HOST, PORT))
            print(f"Sent data: {data}")  # Log the data sent for debugging

            # Increment time_step for the next update
            time_step += 0.1

            # Wait before sending the next update (e.g., 10 Hz for testing)
            time.sleep(0.1)


# Step 6: Run the sender
if __name__ == "__main__":
    send_data()
