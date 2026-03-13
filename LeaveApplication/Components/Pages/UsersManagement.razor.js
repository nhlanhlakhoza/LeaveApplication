// Task Management JavaScript
export function showRefreshAnimation(buttonId) {
    const button = document.getElementById(buttonId);
    if (button) {
        const originalHTML = button.innerHTML;
        button.innerHTML = '<i class="fas fa-sync-alt fa-spin"></i> Refreshing...';
        button.disabled = true;

        setTimeout(() => {
            button.innerHTML = originalHTML;
            button.disabled = false;
        }, 1000);
    }
}

export function exportTasks() {
    // Simulate export process
    console.log('Exporting tasks data...');

    // Create CSV content
    const tasks = Array.from(document.querySelectorAll('tbody tr')).map(row => {
        const cells = row.querySelectorAll('td');
        return {
            title: cells[0].querySelector('.task-title').textContent,
            description: cells[0].querySelector('.task-description').textContent,
            agent: cells[1].querySelector('.agent-name').textContent,
            dueDate: cells[2].textContent,
            priority: cells[3].querySelector('.priority-badge').textContent,
            status: cells[4].querySelector('select').value
        };
    });

    // Convert to CSV
    const headers = ['Title', 'Description', 'Agent', 'Due Date', 'Priority', 'Status'];
    const csvContent = [
        headers.join(','),
        ...tasks.map(task => [
            `"${task.title}"`,
            `"${task.description}"`,
            `"${task.agent}"`,
            `"${task.dueDate}"`,
            `"${task.priority}"`,
            `"${task.status}"`
        ].join(','))
    ].join('\n');

    // Create and download file
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'tasks_export.csv';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);

    alert('Tasks exported successfully! CSV file downloaded.');
}

export function initialize() {
    console.log('Task Management initialized');
}
