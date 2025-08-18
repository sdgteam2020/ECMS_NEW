let allLogs = [];
let filteredLogs = [];
let currentPage = 1;
let pageSize = 50;

let selectedDomain = "";
let selectedFeedback = "all";
let selectedFromDate = "";
let selectedToDate = "";




// Load logs on page load
document.addEventListener("DOMContentLoaded", () => {
  loadLogs();

  document.getElementById("from-date").addEventListener("change", filterLogsByDateRange);
  document.getElementById("to-date").addEventListener("change", filterLogsByDateRange);
  document.getElementById("feedbackFilter").addEventListener("change", e => {
    selectedFeedback = e.target.value;
    applyCombinedFilters();
  });
});

async function loadLogs() {
  try {
    const res = await fetch("/api/query-logs/");
    const data = await res.json();
    allLogs = data.logs || [];
    filteredLogs = [...allLogs];

    populateDomainDropdown(allLogs);
    applyCombinedFilters();
        renderFeedbackColumnChart(allLogs);
        renderYearlyQueryBreakup(allLogs); // This should be called inside loadLogs

  } catch (err) {
    console.error("Error loading logs:", err);
  }
}

function populateDomainDropdown(logs) {
  const select = document.getElementById("domainSelect");
  select.innerHTML = `<option value="">All Domains</option>`;

  const domains = [...new Set(logs.map(log => log.domain).filter(Boolean))];

  domains.forEach(domain => {
    const option = document.createElement("option");
    option.value = domain;
    option.textContent = domain;
    select.appendChild(option);
  });

  select.addEventListener("change", function () {
    selectedDomain = this.value;
    applyCombinedFilters();
  });
}

function applyCombinedFilters() {
  filteredLogs = allLogs.filter(log => {
    const domainMatch = !selectedDomain || log.domain === selectedDomain;
    const feedbackMatch = selectedFeedback === "all" || log.feedback === selectedFeedback;

    const logDate = new Date(log.timestamp);
    const from = selectedFromDate ? new Date(selectedFromDate) : null;
    const to = selectedToDate ? new Date(selectedToDate) : null;
    const dateMatch = (!from || logDate >= from) && (!to || logDate <= to);

    return domainMatch && feedbackMatch && dateMatch;
  });

  currentPage = 1;
  renderPaginatedLogs();
}

function filterLogsByDateRange() {
  selectedFromDate = document.getElementById("from-date").value;
  selectedToDate = document.getElementById("to-date").value;
  applyCombinedFilters();
}

function changePageSize() {
  pageSize = parseInt(document.getElementById("pageSize").value);
  currentPage = 1;
  renderPaginatedLogs();
}

function renderPaginatedLogs() {
  const start = (currentPage - 1) * pageSize;
  const end = start + pageSize;
  const currentLogs = filteredLogs.slice(start, end);
  renderLogs(currentLogs);
  renderPaginationControls();
}

function renderLogs(logs) {
  const tableBody = document.getElementById("log-table-body");
  tableBody.innerHTML = "";

  if (logs.length === 0) {
    const noDataRow = document.createElement("tr");
    noDataRow.innerHTML = `<td colspan="5" class="text-center text-muted">No logs available</td>`;
    tableBody.appendChild(noDataRow);
    return;
  }

  logs.forEach(log => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td><input type="checkbox" class="log-checkbox" data-id="${log.id}"></td>
      <td>${log.question || "No question"}</td>
      <td>${log.answer || "No answer"}</td>
      <td>${log.feedback || "-"}</td>
      <td>${log.timestamp || "No timestamp"}</td>
    `;
    tableBody.appendChild(row);
  });
}

function renderPaginationControls() {
  const pagination = document.getElementById("pagination-controls");
  pagination.innerHTML = "";

  const totalPages = Math.ceil(filteredLogs.length / pageSize);
  for (let i = 1; i <= totalPages; i++) {
    const btn = document.createElement("button");
    btn.textContent = i;
    btn.className = `btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'}`;
    btn.onclick = () => {
      currentPage = i;
      renderPaginatedLogs();
    };
    pagination.appendChild(btn);
  }
}

function toggleSelectAll() {
  const selectAllCheckbox = document.getElementById("select-all");
  const checkboxes = document.querySelectorAll(".log-checkbox");
  checkboxes.forEach(cb => cb.checked = selectAllCheckbox.checked);
}

function getSelectedLogs() {
  const selected = [];
  document.querySelectorAll(".log-checkbox:checked").forEach(cb => {
    const id = cb.dataset.id;
    const log = allLogs.find(log => log.id == id);
    if (log) selected.push(log);
  });
  return selected;
}

function exportToCSV() {
  const selectedLogs = getSelectedLogs();
  const logsToExport = selectedLogs.length ? selectedLogs : filteredLogs;

  if (!logsToExport.length) {
    alert("No logs to export.");
    return;
  }

  const headers = ["Question", "Answer", "Feedback", "Timestamp"];
  const rows = logsToExport.map(log => [
    `"${log.question?.replace(/"/g, '""') || ''}"`,
    `"${log.answer?.replace(/"/g, '""') || ''}"`,
    `"${log.feedback || '-'}"`,
    `"${log.timestamp || '-'}"`
  ]);

  const csvContent = [headers.join(","), ...rows.map(r => r.join(","))].join("\n");

  const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "logs_export.csv";
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
}

function resetAllFilters() {
  selectedDomain = "";
  selectedFeedback = "all";
  selectedFromDate = "";
  selectedToDate = "";

  document.getElementById("domainSelect").value = "";
  document.getElementById("feedbackFilter").value = "all";
  document.getElementById("from-date").value = "";
  document.getElementById("to-date").value = "";
  document.getElementById("pageSize").value = "50";
  document.getElementById("select-all").checked = false;

  pageSize = 50;
  applyCombinedFilters();
}
function resetAllFilters() {
  selectedDomain = "";
  selectedFeedback = "all";
  selectedFromDate = "";
  selectedToDate = "";

  document.getElementById("domainSelect").value = "";
  document.getElementById("feedbackFilter").value = "all";
  document.getElementById("from-date").value = "";
  document.getElementById("to-date").value = "";
  document.getElementById("pageSize").value = "50";
  document.getElementById("select-all").checked = false;

  pageSize = 50;
  applyCombinedFilters();
}
function filterLogsByFeedback() {
  const feedbackSelect = document.getElementById("feedbackFilter");
  selectedFeedback = feedbackSelect ? feedbackSelect.value : "all";
  applyCombinedFilters();
}
function renderFeedbackColumnChart(logs) {
  const counts = {
    like: 0,
    dislike: 0,
    none: 0
  };

  logs.forEach(log => {
    if (log.feedback === "like") counts.like++;
    else if (log.feedback === "dislike") counts.dislike++;
    else counts.none++;
  });

  const options = {
    chart: {
      type: 'bar',
      height: 350
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '45%'
      }
    },
    dataLabels: {
      enabled: true
    },
    series: [{
      name: 'Feedback Count',
      data: [counts.like, counts.dislike, counts.none]
    }],
    xaxis: {
      categories: ['Like', 'Dislike', 'No Feedback']
    },
    colors: ['#5D87FF', '#49BEFF', '#a0c2f1ff']
  };

  if (window.feedbackChart) window.feedbackChart.destroy();
  window.feedbackChart = new ApexCharts(document.querySelector("#chart-bar"), options);
  window.feedbackChart.render();
}

function renderYearlyQueryBreakup(logs) {
  const total = logs.length;
  const withFeedback = logs.filter(log => log.feedback === "like" || log.feedback === "dislike").length;
  const percentWithFeedback = total > 0 ? Math.round((withFeedback / total) * 100) : 0;

  const breakupEl = document.querySelector("#breakup");
  if (!breakupEl) {
    console.warn("📉 #breakup element not found. Skipping chart rendering.");
    return;
  }

  // Set text content
  document.getElementById("totalQueryCount").textContent = `${total} Queries`;
  document.getElementById("percentageFeedback").textContent = `+${percentWithFeedback}%`;

  // Destroy any existing chart first (optional safety)
  breakupEl.innerHTML = "";

  const options = {
    series: [percentWithFeedback],
    chart: {
      height: 150,
      type: 'radialBar'
    },
    plotOptions: {
      radialBar: {
        hollow: { size: '50%' },
        dataLabels: {
          name: { show: false },
          value: {
            fontSize: '20px',
            formatter: val => `${val}%`
          }
        }
      }
    },
    labels: ['Feedback Given'],
    colors: ['#5D87FF'],
  };

  const chart = new ApexCharts(breakupEl, options);
  chart.render();
}
 function renderPdfChart(growthPercent) {
  const chartDiv = document.querySelector("#earning");
  if (!chartDiv) {
    console.error("Element #earning not found");
    return;
  }

  const options = {
    chart: {
      height: 120,  // increase from 20 for visibility
      type: 'radialBar',
      toolbar: { show: false },
      sparkline: { enabled: true }  // if you want compact style
    },
    series: [growthPercent],
    labels: ["Monthly Upload Growth"],
    plotOptions: {
      radialBar: {
        hollow: {
          size: '60%'
        },
        dataLabels: {
          show: true,
          name: {
            show: false
          },
          value: {
            fontSize: '18px',
            fontWeight: 600,
            formatter: function (val) {
              return `${val}%`;
            }
          }
        }
      }
    },
    colors: ['#5D87FF']
  };

  if (window.pdfChart) window.pdfChart.destroy();

  window.pdfChart = new ApexCharts(chartDiv, options);
  window.pdfChart.render();
}
function updatePdfStats() {
  fetch('/api/store-pdf/')
    .then(res => res.json())
    .then(data => {
      const totalPDFs = data.length;
      document.getElementById("pdfCount").textContent = `${totalPDFs}`;

      const currentMonth = new Date().getMonth();
      const uploadsThisMonth = data.filter(pdf => {
        const uploadedMonth = new Date(pdf.created_at).getMonth();
        return uploadedMonth === currentMonth;
      });

      const growth = totalPDFs > 0
        ? Math.round((uploadsThisMonth.length / totalPDFs) * 100)
        : 0;

      document.getElementById("pdfGrowth").textContent = `+${growth}%`;

      renderPdfChart(growth);
    })
    .catch(err => {
      console.error("Failed to fetch PDF data:", err);
      document.getElementById("pdfCount").textContent = "Error";
      document.getElementById("pdfGrowth").textContent = "";
    });
}
document.addEventListener("DOMContentLoaded", () => {
  console.log("DOM fully loaded, calling updatePdfStats");
  updatePdfStats();
});
