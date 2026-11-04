export const sidebarPreferenceKey = "educon:sidebar-open";

// Runs synchronously in <head>, before the sidebar or its content can be painted.
// Keep the default identical to the HTML attribute and the React preference fallback.
export const sidebarInitScript = `(()=>{try{document.documentElement.dataset.sidebarOpen=localStorage.getItem(${JSON.stringify(sidebarPreferenceKey)})==="false"?"false":"true"}catch{}})();`;
