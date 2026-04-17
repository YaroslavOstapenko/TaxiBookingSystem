import React, { useState, useEffect } from "react";
import axios from "axios";

// Icons from lucide-react library
import {
    Plus,
    RefreshCw,
    Rocket,
    Trash2,
    Check,
    X,
    Phone,
    MapPin
} from "lucide-react";

// API Base URL - adjust this if your backend port changes
const API = "http://localhost:5059/api/bookings";

export default function BookingPage() {

    // --- State Management ---
    const [showForm, setShowForm] = useState(false); // Controls the visibility of the "Create" form
    const [bookingId, setBookingId] = useState("");
    const [result, setResult] = useState(null);
    const [orders, setOrders] = useState([]);        // Stores the list of bookings from the database
    const [running, setRunning] = useState(false);  // Indicates if a simulation is currently active

    // Sample data for locations
    const cities = [
        "Rostock", "Berlin", "Hamburg",
        "Munich", "Cologne", "Frankfurt", "Leipzig"
    ];

    // --- Inline Styles ---
    const fieldStyle = {
        width: "100%",
        padding: "10px",
        marginBottom: "12px",
        borderRadius: "6px",
        border: "1px solid #ddd",
        boxSizing: "border-box"
    };

    const cardStyle = {
        border: "1px solid #e5e5e5",
        borderRadius: "10px",
        padding: "15px",
        marginBottom: "15px",
        background: "#fff"
    };

    // State for the creation form
    const [form, setForm] = useState({
        userPhone: "",
        pickupLocation: "",
        dropLocation: ""
    });

    const tableStyle = {
        width: "100%",
        borderCollapse: "collapse",
        background: "#fff",
        borderRadius: "10px",
        overflow: "hidden"
    };

    const thStyle = {
        textAlign: "left",
        padding: "12px",
        background: "#f5f5f5",
        fontWeight: "600",
        fontSize: "14px",
        borderBottom: "1px solid #e5e5e5"
    };

    const tdStyle = {
        padding: "12px",
        borderBottom: "1px solid #f0f0f0",
        fontSize: "14px"
    };

    const actionCellStyle = {
        display: "flex",
        gap: "8px",
        alignItems: "center"
    };

    // --- Side Effects ---
    useEffect(() => {
        // Initial fetch when the component mounts
        fetchAll();

        // Set up polling to refresh the data every 3 seconds
        const interval = setInterval(() => {
            fetchAll();
        }, 3000);

        // Cleanup interval when the component is unmounted
        return () => clearInterval(interval);
    }, []);

    // --- API Methods ---

    /**
     * Sends a POST request to create a new booking record.
     */
    const createBooking = async () => {
        try {
            if (!form.userPhone || !form.pickupLocation || !form.dropLocation) {
                alert("Fill all fields");
                return;
            }

            const res = await axios.post(`${API}/create`, form);

            setResult(res.data);
            setShowForm(false); // Close form on success
            setForm({ userPhone: "", pickupLocation: "", dropLocation: "" }); // Reset form
            fetchAll();

        } catch (err) {
            console.log("CREATE ERROR:", err.response?.data);
        }
    };

    /**
     * Retrieves the latest bookings from the backend.
     */
    const fetchAll = async () => {
        try {
            const res = await axios.get(`${API}/all`);
            setOrders(res.data);
        } catch (err) {
            console.log("FETCH ERROR:", err.response?.data);
        }
    };

    /**
     * Transitions a booking status to "CANCELLED".
     */
    const cancelBooking = async (id) => {
        try {
            const res = await axios.post(`${API}/${id}/cancel`);
            console.log("CANCEL OK:", res.data);
            fetchAll();
        } catch (err) {
            console.log("CANCEL ERROR:", err.response?.data);
            alert(err.response?.data?.message || "Cancel failed");
        }
    };

    /**
     * Transitions a booking status to "CONFIRMED".
     */
    const confirmBooking = async (id) => {
        try {
            const res = await axios.post(`${API}/${id}/confirm`);
            console.log("CONFIRM OK:", res.data);
            fetchAll();
        } catch (err) {
            console.log("CONFIRM ERROR:", err.response?.data);
            alert(err.response?.data?.message || "Confirm failed");
        }
    };

    /**
     * Removes a booking record from the database.
     */
    const deleteBooking = async (id) => {
        try {
            const res = await axios.delete(`${API}/${id}`);
            console.log("DELETE OK:", res.data);
            fetchAll();
        } catch (err) {
            console.log("DELETE ERROR:", err.response?.data);
            alert(err.response?.data?.message || "Delete failed");
        }
    };

    /**
     * Triggers the backend simulation logic to generate random load.
     */
    const runSimulation = async () => {
        setRunning(true);
        try {
            const res = await axios.post(`${API}/simulate`);
            console.log("SIMULATION OK:", res.data);
            await fetchAll();
        } catch (err) {
            console.log("SIMULATION ERROR:", err.response?.data);
            alert(err.response?.data?.message || "Simulation failed");
        } finally {
            setRunning(false);
        }
    };

    // --- Render ---
    return (
        <div style={{ padding: 24, fontFamily: "Arial", background: "#f5f5f5", minHeight: "100vh" }}>

            <h1>Taxi Booking Dashboard</h1>

            {/* Global Actions */}
            <div style={{ display: "flex", gap: 10, marginBottom: 20 }}>
                <button onClick={() => setShowForm(true)}>
                    <Plus size={16} /> Create New
                </button>

                <button onClick={fetchAll}>
                    <RefreshCw size={16} /> Refresh Data
                </button>

                <button onClick={runSimulation} disabled={running}>
                    <Rocket size={16} /> {running ? "Simulating..." : "Run Simulation"}
                </button>
            </div>

            {/* Conditional Rendering: Creation Form */}
            {showForm && (
                <div style={cardStyle}>
                    <h3>New Booking Details</h3>

                    <input
                        style={fieldStyle}
                        placeholder="User Phone Number"
                        value={form.userPhone}
                        onChange={(e) =>
                            setForm({ ...form, userPhone: e.target.value })
                        }
                    />

                    <select
                        style={fieldStyle}
                        value={form.pickupLocation}
                        onChange={(e) =>
                            setForm({ ...form, pickupLocation: e.target.value })
                        }
                    >
                        <option value="">Select Pickup City</option>
                        {cities.map(c => (
                            <option key={c} value={c}>{c}</option>
                        ))}
                    </select>

                    <select
                        style={fieldStyle}
                        value={form.dropLocation}
                        onChange={(e) =>
                            setForm({ ...form, dropLocation: e.target.value })
                        }
                    >
                        <option value="">Select Dropoff City</option>
                        {cities.map(c => (
                            <option key={c} value={c}>{c}</option>
                        ))}
                    </select>

                    <div style={{ display: "flex", gap: 10 }}>
                        <button onClick={createBooking}>Submit</button>
                        <button onClick={() => setShowForm(false)}>Discard</button>
                    </div>
                </div>
            )}

            {/* Data Table */}
            <div style={cardStyle}>
                <h2>Recent Orders</h2>

                <table style={tableStyle}>
                    <thead>
                        <tr>
                            <th style={thStyle}>Booking ID</th>
                            <th style={thStyle}>Customer</th>
                            <th style={thStyle}>From</th>
                            <th style={thStyle}>To</th>
                            <th style={thStyle}>Status</th>
                            <th style={thStyle}>Management</th>
                        </tr>
                    </thead>

                    <tbody>
                        {orders.map(o => (
                            <tr key={o.id} style={{ transition: "0.2s" }}>

                                <td style={tdStyle}>
                                    {o.id}
                                </td>

                                <td style={tdStyle}>
                                    {o.userPhone}
                                </td>

                                <td style={tdStyle}>
                                    {o.pickupLocation}
                                </td>

                                <td style={tdStyle}>
                                    {o.dropLocation}
                                </td>

                                <td style={tdStyle}>
                                    <span style={{
                                        padding: "4px 8px",
                                        borderRadius: "6px",
                                        fontSize: "12px",
                                        fontWeight: "bold",
                                        background:
                                            o.status === "PENDING" ? "#fff3cd" :
                                                o.status === "CONFIRMED" ? "#d4edda" :
                                                    "#f8d7da",
                                        color: "#333"
                                    }}>
                                        {o.status}
                                    </span>
                                </td>

                                <td style={tdStyle}>
                                    <div style={actionCellStyle}>
                                        <button onClick={() => confirmBooking(o.id)}>
                                            Confirm
                                        </button>

                                        <button onClick={() => cancelBooking(o.id)}>
                                            Cancel
                                        </button>

                                        <button
                                            onClick={() => deleteBooking(o.id)}
                                            style={{ color: "red" }}
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </td>

                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}