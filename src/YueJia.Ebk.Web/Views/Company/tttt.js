const { reactive } = Vue;

export default {
    setup() {
        const sate = reactive({
            openAddDialog: false,
            title: `Default Title`,
        });
        const tttx = () => {
            sate.title = `������˾`;
            alert("rrrrrrrrrrr");
        };
        return { sate, tttx };
    },
};
