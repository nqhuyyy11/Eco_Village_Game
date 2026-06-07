// Game State
const state = {
    theme: 'abandoned',
    resources: { coins: 4250, ecoPoints: 0, energy: 88, wood: 10, stone: 5 },
    activeTool: 'clear-trash',
    quests: [
        { id: 1, text: "Dọn dẹp rác thải trên đảo", target: 3, current: 0, completed: false },
        { id: 2, text: "Sửa chữa nhà cổ", target: 2, current: 0, completed: false }
    ]
};

const TOOLS = {
    'clear-trash': { name: 'Dọn Rác', energy: 10, cost: {}, reward: { coins: 20, wood: 2 } },
    'repair-ruin': { name: 'Sửa Cũ', energy: 20, cost: { wood: 5 }, reward: { ecoPoints: 15, coins: 50 } },
    'build-homestay': { name: 'Homestay', energy: 15, cost: { coins: 80, wood: 10, stone: 5 }, reward: { ecoPoints: 30 } },
    'build-solar': { name: 'Năng Lượng', energy: 10, cost: { coins: 120, stone: 8 }, reward: { ecoPoints: 50 } },
    'build-cafe': { name: 'Cà Phê', energy: 15, cost: { coins: 100, wood: 15 }, reward: { ecoPoints: 40 } },
    'build-well': { name: 'Giếng Cổ', energy: 10, cost: { coins: 50, stone: 10 }, reward: { ecoPoints: 20 } }
};

// DOM Elements
const el = {
    coins: document.getElementById('val-coins'),
    energyBar: document.getElementById('energy-bar'),
    energyVal: document.getElementById('val-energy'),
    wood: document.getElementById('val-wood'),
    stone: document.getElementById('val-stone'),
    eco: document.getElementById('val-eco'),
    alert: document.getElementById('alert-box'),
    grid: document.getElementById('map-grid-container')
};

// Initialize Grid (just transparent clickable boxes)
function initGrid() {
    el.grid.innerHTML = '';
    for(let i=0; i<24; i++) {
        const cell = document.createElement('div');
        cell.onclick = () => handleMapClick(i);
        el.grid.appendChild(cell);
    }
}

// Update UI
function updateHUD() {
    // Format coins with commas
    el.coins.innerText = state.resources.coins.toLocaleString();
    
    // Animate energy bar width
    el.energyBar.style.width = `${state.resources.energy}%`;
    el.energyVal.innerText = `${state.resources.energy}%`;
    
    // Update mini bar
    el.wood.innerText = state.resources.wood;
    el.stone.innerText = state.resources.stone;
    el.eco.innerText = state.resources.ecoPoints;

    // Change energy bar color if low
    if (state.resources.energy < 20) {
        el.energyBar.style.background = '#dc2626'; // Red warning
    } else {
        el.energyBar.style.background = 'var(--energy)'; // Normal yellow
    }
}

// Show Alert
function showAlert(msg) {
    el.alert.innerText = msg;
    el.alert.style.display = 'block';
    setTimeout(() => { el.alert.style.display = 'none'; }, 2000);
}

// Map Click Logic (Core interaction)
function handleMapClick(index) {
    const tool = TOOLS[state.activeTool];
    
    // Check energy
    if (state.resources.energy < tool.energy) {
        showAlert("❌ Không đủ năng lượng! Hãy nghỉ ngơi.");
        return;
    }

    // Check resources
    for (let res in tool.cost) {
        if (state.resources[res] < tool.cost[res]) {
            showAlert(`❌ Không đủ nguyên liệu (${res})!`);
            return;
        }
    }

    // Apply costs
    state.resources.energy -= tool.energy;
    for (let res in tool.cost) {
        state.resources[res] -= tool.cost[res];
    }

    // Apply rewards
    if (tool.reward) {
        for (let res in tool.reward) {
            state.resources[res] += tool.reward[res];
        }
    }

    // Close panel automatically after interaction
    closePanel();
    
    // Feedback
    showAlert(`✅ Thực hiện: ${tool.name} ( -${tool.energy}⚡ )`);
    updateHUD();
}

// Panel Switching Logic
function switchPanel(panelId) {
    // Close all panels
    document.querySelectorAll('.slide-panel').forEach(p => p.classList.remove('active'));
    // Deactivate all bottom buttons
    document.querySelectorAll('.action-btn').forEach(b => b.classList.remove('active'));
    
    // Open target panel
    document.getElementById(`panel-${panelId}`).classList.add('active');
    document.getElementById(`btn-${panelId}`).classList.add('active');
}

function closePanel() {
    document.querySelectorAll('.slide-panel').forEach(p => p.classList.remove('active'));
    document.querySelectorAll('.action-btn').forEach(b => b.classList.remove('active'));
}

// Tool Selection
function selectTool(buttonElement) {
    document.querySelectorAll('.tool-card').forEach(b => b.classList.remove('active'));
    buttonElement.classList.add('active');
    state.activeTool = buttonElement.dataset.tool;
    showAlert(`Đã chọn công cụ: ${TOOLS[state.activeTool].name}`);
}

window.onload = () => {
    initGrid();
    updateHUD();
    
    // Tự động hồi năng lượng theo thời gian (1 năng lượng mỗi 2 giây)
    setInterval(() => {
        if (state.resources.energy < 100) {
            state.resources.energy = Math.min(100, state.resources.energy + 1);
            updateHUD();
        }
    }, 2000);
};
