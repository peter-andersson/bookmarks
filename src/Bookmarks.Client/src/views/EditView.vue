<script setup lang="ts">
  import Bookmark from "@/components/Bookmark.vue";
  import ErrorMessage from "@/components/ErrorMesage.vue";

  import router from '@/router';
  import { useRoute } from "vue-router";
  import { watch } from "vue";
  import { ref } from 'vue'
  import type { Ref } from 'vue'
  import type { BookmarkModel} from "@/models";
  import { api } from "@/services/api";


  let loading : Ref<boolean> = ref(false);
  let edit : Ref<boolean> = ref(false);
  let saving : Ref<boolean> = ref(false);
  let error : string = "";
  let bookmark : BookmarkMode | null;

  const route = useRoute();

  watch(
      () => route.params,
      async () => {
        await fetchData();
      },
      {immediate: true}
  );

  async function fetchData() {
    bookmark = null;
    loading.value = true;
    edit.value = false;

    try {
      bookmark = await api.getBookmark(route.params.id);
      loading.value = false;
      if (!bookmark) {
        error = "Failed to get bookmark";
      } else {
        edit.value = true;
      }
    } catch (err : any) {
      loading.value = false;
      error = err.toString();
    }
  }

  function cancel() {
    router.push('/');
  }

  async function save() {
    saving.value = true;

    console.log(bookmark);

    try {
      const result = await api.updateBookmark(bookmark);
      if (result) {
        await router.push('/');
      } else {
        saving.value = false;
        error = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      error = err.toString();
    }
  }
</script>

<template>
  <h1>Edit bookmark</h1>

  <div v-if="loading">Loading...</div>

  <ErrorMessage :error=error />

  <Bookmark v-if="edit" @ok="save" @cancel="cancel" button-text="Save" :bookmark=bookmark :progress=saving />
</template>
