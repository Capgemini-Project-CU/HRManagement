const sidebar = document.getElementById("sidebar");
const toggle = document.querySelector("[data-sidebar-toggle]");

if (sidebar && toggle) {
  toggle.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}
