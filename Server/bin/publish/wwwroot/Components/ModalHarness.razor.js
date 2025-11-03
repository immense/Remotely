/**
 * 
 * @param {HTMLElement} modal
 */
export function showModal(modal) {
    const modalApi = new bootstrap.Modal(modal);
    modalApi.show();
}