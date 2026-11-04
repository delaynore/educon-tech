export const themePreferenceKey = "educon:dark-mode";

// Apply the saved choice before the first paint; dark matches the existing layout default.
export const themeInitScript = `(()=>{try{document.documentElement.classList.toggle("dark",localStorage.getItem(${JSON.stringify(themePreferenceKey)})!=="false")}catch{}})();`;
