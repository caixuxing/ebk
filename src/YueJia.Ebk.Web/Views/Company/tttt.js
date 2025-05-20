const { reactive } = Vue;

export default {
    setup() {
        const sate = reactive({
            openAddDialog: false,
            title: `Default Title`,
        });
        const tttx = () => {
            sate.title = `新增公司`;
            alert("rrrrrrrrrrr");
        };
        return { sate, tttx };
    },
};
