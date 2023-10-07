<script setup lang="ts">
  import Bookmark from "@/components/Bookmark.vue";
  import ErrorMessage from "@/components/ErrorMesage.vue";

  import router from '@/router';
  import type { BookmarkModel } from "@/models";
  import { api } from "@/services/api";

  let adding = false;
  let error : string = "";
  let bookmark : BookmarkModel = {
    "id": 0,
    "url": "",
    "tags": [],
    "title": null,
    "description": null,
  };

  function cancel() {
    router.push('/');
  }

  async function add() {
    adding = true;

    try {
      const result = await api.addBookmark(bookmark);
      if (result) {
        await router.push('/');
      }
      else {
        adding = false;
        error = "Failed to add bookmark";
      }
    } catch (err : any) {
      adding = false;
      error = err.toString();
    }
  }
</script>

<template>
  <h1>Add bookmark</h1>

  <ErrorMessage :error=error />

  <Bookmark @ok="add" @cancel="cancel" button-text="Add" :bookmark=bookmark :progress=adding />
</template>
