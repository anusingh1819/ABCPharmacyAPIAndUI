const API = 'http://localhost:5211'; // update port if different

async function fetchMeds(search = '') {
  const url = new URL(API + '/api/medicines');
  if (search) url.searchParams.append('search', search);
  const res = await fetch(url);
  return await res.json();
}

async function fetchSales() {
  const res = await fetch(API + '/api/sales');
  return await res.json();
}

function daysUntil(dateStr) {
  const d = new Date(dateStr);
  const now = new Date();
  const diff = d - now;
  return Math.ceil(diff / (1000*60*60*24));
}

function renderMeds(meds) {
  const tbody = document.querySelector('#medTable tbody');
  tbody.innerHTML = '';
  meds.forEach(m => {
    const days = daysUntil(m.expiryDate);
    const tr = document.createElement('tr');
    if (days <= 30) tr.classList.add('red');
    else if (m.quantity < 10) tr.classList.add('yellow');

    tr.innerHTML = `
      <td>${m.fullName}</td>
      <td>${m.brand}</td>
      <td>${new Date(m.expiryDate).toLocaleDateString()}</td>
      <td>${m.quantity}</td>
      <td>${Number(m.price).toFixed(2)}</td>
      <td class="actions">
        <button data-id="${m.id}" class="sellBtn">Sell</button>
      </td>
    `;
    tbody.appendChild(tr);
  });

  document.querySelectorAll('.sellBtn').forEach(btn => btn.onclick = async (e) => {
    const id = e.target.dataset.id;
    const qty = prompt('Enter quantity to sell', '1');
    if (!qty) return;
    const q = Number(qty);
    if (isNaN(q) || q <= 0) { alert('Invalid'); return; }
    const res = await fetch(`${API}/api/medicines/${id}/sell`, {
      method: 'POST',
      headers: {'Content-Type':'application/json'},
      body: JSON.stringify({ quantity: q })
    });
    if (!res.ok) {
      const err = await res.json();
      alert(err.error || 'Sell failed');
    }
    await reload();
  });
}

function renderSales(sales) {
  const ul = document.getElementById('salesList');
  ul.innerHTML = '';
  sales.forEach(s => {
    const li = document.createElement('li');
    li.textContent = `${s.medicineName} — ${s.quantitySold} units @ ${Number(s.salePricePerUnit).toFixed(2)} on ${new Date(s.soldAt).toLocaleString()}`;
    ul.appendChild(li);
  });
}

async function reload(search = '') {
  const meds = await fetchMeds(search);
  renderMeds(meds);
  const sales = await fetchSales();
  renderSales(sales);
}

document.getElementById('searchForm').onsubmit = (e) => {
  e.preventDefault();
  const q = document.getElementById('searchInput').value.trim();
  reload(q);
};
document.getElementById('clearBtn').onclick = () => {
  document.getElementById('searchInput').value = '';
  reload();
};

document.getElementById('addForm').onsubmit = async (e) => {
  e.preventDefault();
  const f = e.target;
  const payload = {
    fullName: f.fullName.value,
    brand: f.brand.value,
    expiryDate: f.expiryDate.value,
    quantity: Number(f.quantity.value),
    price: Number(Number(f.price.value).toFixed(2)),
    notes: f.notes.value
  };
  const res = await fetch(API + '/api/medicines', {
    method: 'POST',
    headers: {'Content-Type':'application/json'},
    body: JSON.stringify(payload)
  });
  if (res.ok) {
    f.reset();
    reload();
  } else {
    alert('Add failed');
  }
};

reload();
