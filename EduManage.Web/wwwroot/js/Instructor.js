const cardsContainer = document.querySelector(".teachers-grid");
const buttons = document.querySelectorAll(".sort button");

// بنجيب الكروت مرة واحدة
const cards = Array.from(document.querySelectorAll(".teacher-card"));

buttons.forEach(button => {

    button.addEventListener("click", () => {

        // تغيير الزر النشط
        buttons.forEach(btn => btn.classList.remove("active"));
        button.classList.add("active");

        let sortedCards = [...cards];

        switch (button.textContent.trim()) {

            case "Students":
                sortedCards.sort((a, b) => {
                    return Number(b.dataset.students) - Number(a.dataset.students);
                });
                break;

            case "Top Rated":
                sortedCards.sort((a, b) => {
                    return Number(b.dataset.rating) - Number(a.dataset.rating);
                });
                break;

            case "Courses":
                sortedCards.sort((a, b) => {
                    return Number(b.dataset.courses) - Number(a.dataset.courses);
                });
                break;
        }

        // إعادة ترتيب الكروت
        cardsContainer.innerHTML = "";

        sortedCards.forEach(card => {
            cardsContainer.appendChild(card);
        });

    });

});